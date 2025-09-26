using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceBackend.Data;

namespace EcommerceBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public InvoicesController(AppDbContext db) => _db = db;

        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var subject = User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            return !string.IsNullOrWhiteSpace(subject) && Guid.TryParse(subject, out userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyInvoices()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var inv = await _db.Invoices.Where(i => i.UserId == userId).ToListAsync();
            return Ok(inv);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var inv = await _db.Invoices
                .Include(i => i.Items)
                .SingleOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (inv == null) return NotFound();
            return Ok(inv);
        }
    }
}
