using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceTrackerAPIWebApp.Models;
using SpaceTrackerApp.Models;
using System.Security.Cryptography;
using System.Text;

namespace SpaceTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SpaceTrackerContext _context;

        public AuthController(SpaceTrackerContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) ||
                string.IsNullOrWhiteSpace(req.Email) ||
                string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Заповніть всі поля");

            if (await _context.Users.AnyAsync(u => u.Email == req.Email))
                return BadRequest("Користувач з таким email вже існує");

            var user = new User
            {
                Username = req.Username,
                Email = req.Email,
                PasswordHash = HashPassword(req.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Username, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) ||
                string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Заповніть всі поля");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null || user.PasswordHash != HashPassword(req.Password))
                return Unauthorized("Невірний email або пароль");

            return Ok(new { user.Id, user.Username, user.Email });
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }

    public class AuthRequest
    {
        public string? Username { get; set; }
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}