using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EcommerceBackend.Data;
using EcommerceBackend.Models;
using EcommerceBackend.Services;
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
            var email = (dto.Email ?? string.Empty).Trim();
            var password = (dto.Password ?? string.Empty).Trim();
            var fullName = string.IsNullOrWhiteSpace(dto.FullName) ? null : dto.FullName.Trim();

            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
                return BadRequest(new { error = "Correo electrónico inválido" });

            if (password.Length < 6)
                return BadRequest(new { error = "La contraseña debe tener al menos 6 caracteres" });

            var normalizedEmail = email.ToLowerInvariant();
            var defaultRole = User.FindFirstValue(ClaimTypes.Role) ?? "customer";

            if (await _db.Users.AsNoTracking().AnyAsync(u => u.NormalizedEmail == normalizedEmail))
                return Conflict(new { error = "El correo electrónico ya está registrado" });

            var user = new User
            {
                Email = email,
                NormalizedEmail = normalizedEmail,
                FullName = fullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = defaultRole
            };

            await _db.Users.AddAsync(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await _db.Users.AsNoTracking().AnyAsync(u => u.NormalizedEmail == normalizedEmail))
                    return Conflict(new { error = "El correo electrónico ya está registrado" });

                throw;
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var email = (dto.Email ?? string.Empty).Trim();
            var password = (dto.Password ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return BadRequest(new { message = "Correo y contraseña son obligatorios" });

            var normalizedEmail = email.ToLowerInvariant();

            var user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
            if (user == null)
                return Unauthorized(new { message = "Usuario no encontrado" });

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
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
