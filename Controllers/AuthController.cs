using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EcommerceBackend.Data;
using EcommerceBackend.Models;
using EcommerceBackend.Services;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext db, TokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = dto.Email.Trim();
            var normalizedEmail = email.ToLowerInvariant();

            if (await _db.Users.AsNoTracking().AnyAsync(u => u.NormalizedEmail == normalizedEmail))
                return BadRequest(new { error = "Email already registered" });

            var password = (dto.Password ?? string.Empty).Trim();
            if (password.Length < 6)
                return BadRequest(new { error = "La contraseña debe tener al menos 6 caracteres" });

            var user = new User
            {
                Email = email,
                NormalizedEmail = normalizedEmail,
                FullName = string.IsNullOrWhiteSpace(dto.FullName) ? null : dto.FullName.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = dto.Email.Trim();
            var normalizedEmail = email.ToLowerInvariant();

            var user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
            if (user == null)
                return Unauthorized(new { message = "Usuario no encontrado" });

            var password = (dto.Password ?? string.Empty).Trim();

            if (password.Length == 0 || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Unauthorized(new { message = "Contraseña incorrecta" });

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token });
        }
    }

    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string? FullName { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
