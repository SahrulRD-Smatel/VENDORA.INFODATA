using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VENDORA.INFODATA.Data;
using VENDORA.INFODATA.Services;
using Microsoft.AspNetCore.Identity.Data;
using VENDORA.INFODATA.Models;
using RegisterRequestDto = VENDORA.INFODATA.Dto.RegisterRequest;
using System.Security.Claims;

namespace VENDORA.INFODATA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, AuthService authService, IConfiguration configuration)
        {
            _context = context;
            _authService = authService;
            _configuration = configuration;
        }

        // 🔹 REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (await _context.Clients.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email sudah digunakan" });
            }

            var hashedPassword = _authService.HashPassword(request.Password); // 🔐 Hash password

            var user = new Client
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Role = "Client",
                PasswordHash = hashedPassword // Simpan password yang sudah di-hash
            };

            _context.Clients.Add(user);
            await _context.SaveChangesAsync();

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token, user });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Clients
                    .Where(u => u.Email == request.Email)
                    .FirstOrDefaultAsync();

                if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash)) // 🔐 Verifikasi password
                {
                    return Unauthorized(new { message = "Email atau password salah" });
                }

                var token = _authService.GenerateJwtToken(user);
                return Ok(new { token, user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal Server Error",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }


        // 🔹 GET CURRENT USER (Protected Route)
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _context.Clients.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound(new { message = "User tidak ditemukan" });
            }

            return Ok(user);
        }
    }

}

