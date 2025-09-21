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

        private Guid UserId => Guid.Parse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub));

        [HttpGet]
        public async Task<IActionResult> GetMyInvoices()
        {
            var inv = await _db.Invoices.Where(i => i.UserId == UserId).ToListAsync();
            return Ok(inv);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            var inv = await _db.Invoices
                .Include(i => i.Items)
                .SingleOrDefaultAsync(i => i.Id == id && i.UserId == UserId);
            if (inv == null) return NotFound();
            return Ok(inv);
        }
    }
}
