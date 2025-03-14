using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VENDORA.INFODATA.Data;

namespace VENDORA.INFODATA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 GET: Data dashboard klien
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var requests = await _context.Requests
                .Where(r => r.ClientId == userId)
                .Include(r => r.Service)
                .Select(r => new
                {
                    r.Id,
                    ServiceName = r.Service.Name,
                    r.Description,
                    r.Status,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(new { requests });
        }
    }
}
