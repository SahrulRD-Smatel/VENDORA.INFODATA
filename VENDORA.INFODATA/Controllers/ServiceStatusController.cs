using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VENDORA.INFODATA.Data;
using VENDORA.INFODATA.Models;

namespace VENDORA.INFODATA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceStatusController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceStatusController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceStatus>>> GetServiceStatuses()
        {
            return await _context.ServiceStatuses.Include(s => s.Request).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceStatus>> GetServiceStatus(int id)
        {
            var status = await _context.ServiceStatuses
                .Include(s => s.Request)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (status == null)
            {
                return NotFound();
            }

            return status;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceStatus>> PostServiceStatus(ServiceStatus status)
        {
            _context.ServiceStatuses.Add(status);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServiceStatus), new { id = status.Id }, status);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceStatus(int id, ServiceStatus status)
        {
            if (id != status.Id)
            {
                return BadRequest();
            }

            _context.Entry(status).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceStatus(int id)
        {
            var status = await _context.ServiceStatuses.FindAsync(id);
            if (status == null)
            {
                return NotFound();
            }

            _context.ServiceStatuses.Remove(status);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
