using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VENDORA.INFODATA.Data;
using VENDORA.INFODATA.Dto;
using VENDORA.INFODATA.Hubs;
using VENDORA.INFODATA.Models;

namespace VENDORA.INFODATA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RequestController(AppDbContext context, IHubContext<NotificationHub> hubContext) // ✅ Tambahkan IHubContext di parameter
        {
            _context = context;
            _hubContext = hubContext; // ✅ Inisialisasi _hubContext dengan hubContext
        }

        // 🔹 POST: Klien mengajukan request layanan
        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] RequestCreateDto requestDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var serviceExists = await _context.Services.AnyAsync(s => s.Id == requestDto.ServiceId);
            if (!serviceExists)
                return BadRequest(new { message = "Layanan tidak ditemukan" });

            var request = new Request
            {
                ClientId = userId,
                ServiceId = requestDto.ServiceId,
                Description = requestDto.Description,
                FilePath = requestDto.FilePath,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Request berhasil dibuat", request });
        }

        // 🔹 GET: Klien melihat status layanan mereka
        [HttpGet("status")]
        public async Task<IActionResult> GetRequestStatus()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var requests = await _context.Requests
                .Where(r => r.ClientId == userId)
                .Include(r => r.Service)
                .Include(r => r.ServiceStatuses)
                .Select(r => new
                {
                    r.Id,
                    ServiceName = r.Service.Name,
                    r.Description,
                    r.Status,
                    StatusHistory = r.ServiceStatuses.OrderByDescending(s => s.UpdatedAt)
                        .Select(s => new { s.Status, s.UpdatedAt })
                })
                .ToListAsync();

            return Ok(new { requests });
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] string status)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = status;
            await _context.SaveChangesAsync();

            // 🔔 Kirim notifikasi realtime ke klien yang membuat request
            await _hubContext.Clients.User(request.ClientId.ToString())
                .SendAsync("ReceiveNotification", "StatusUpdate", request.Id, DateTime.UtcNow);

            return NoContent();
        }
    }
}
