using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VENDORA.INFODATA.Models;
using VENDORA.INFODATA.Hubs;
using VENDORA.INFODATA.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VENDORA.INFODATA.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationsController(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // 🔹 Ambil semua notifikasi berdasarkan userId
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // 🔹 Tambahkan notifikasi + kirim realtime via SignalR
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // 🔔 Kirim notifikasi ke user yang sedang online
            await _hubContext.Clients.User(notification.UserId.ToString())
                .SendAsync("ReceiveNotification", notification.Type, notification.ChatId, notification.CreatedAt);

            return CreatedAtAction(nameof(GetNotifications), new { userId = notification.UserId }, notification);
        }

        // 🔹 Tandai notifikasi sebagai "dibaca"
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🔹 Hapus notifikasi tertentu
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
