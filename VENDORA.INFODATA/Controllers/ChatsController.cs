using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using VENDORA.INFODATA.Data;
using VENDORA.INFODATA.Hubs;
using VENDORA.INFODATA.Models;
using VENDORA.INFODATA.Dto;

namespace VENDORA.INFODATA.Controllers
{
    [Route("api/chats")]
    [ApiController]
    [Authorize] // 🔒 Requires authentication
    public class ChatsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IHubContext<NotificationHub> _hubContext; // ✅ Tambahkan ke constructor

        public ChatsController(AppDbContext context, IHubContext<ChatHub> chatHub, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _chatHub = chatHub;
            _hubContext = hubContext; // ✅ Inisialisasi _hubContext dengan hubContext
        }

        // 🔹 GET: Retrieve messages in a thread
        [HttpGet("thread/{threadId}")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetMessagesByThread(int threadId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var messages = await _context.Chats
                .Where(c => c.ThreadId == threadId && (c.SenderId == userId || c.ReceiverId == userId))
                .OrderBy(c => c.SentAt)
                .ToListAsync();

            foreach (var message in messages)
            {
                if (message.ReceiverId == userId && !message.IsRead)
                {
                    message.IsRead = true;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(messages);
        }

        // 🔹 POST: Send a message & broadcast via SignalR
        [HttpPost]
        public async Task<ActionResult<Chat>> SendMessage([FromBody] ChatMessageDto chatDto)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var threadExists = await _context.ChatThreads.AnyAsync(t => t.Id == chatDto.ThreadId);
            if (!threadExists)
            {
                return BadRequest(new { message = "Thread not found" });
            }

            var chat = new Chat
            {
                ThreadId = chatDto.ThreadId,
                SenderId = senderId,
                ReceiverId = chatDto.ReceiverId,
                ParentMessageId = chatDto.ParentMessageId,
                Message = chatDto.Message,
                FilePath = chatDto.FilePath,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            // 🔹 Kirim pesan ke semua pengguna dalam thread
            await _chatHub.Clients.Group($"thread-{chat.ThreadId}").SendAsync("ReceiveMessage", new
            {
                chat.ThreadId,
                chat.SenderId,
                chat.ReceiverId,
                chat.Message,
                chat.SentAt
            });

            // 🔔 Kirim notifikasi realtime ke penerima
            await _hubContext.Clients.User(chat.ReceiverId.ToString())
                .SendAsync("ReceiveNotification", "NewMessage", chat.ThreadId, chat.SentAt);

            return CreatedAtAction(nameof(GetMessagesByThread), new { threadId = chat.ThreadId }, chat);
        }

        // 🔹 GET: Unread messages count
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadMessagesCount()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int unreadCount = await _context.Chats
                .Where(c => c.ReceiverId == userId && !c.IsRead)
                .CountAsync();

            return Ok(new { unreadCount });
        }
    }
}
