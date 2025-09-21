using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceBackend.Data;
using EcommerceBackend.Models;

namespace EcommerceBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CartController(AppDbContext db) => _db = db;

        private Guid UserId => Guid.Parse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub));

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var items = await _db.CartItems.Where(c => c.UserId == UserId).ToListAsync();
            return Ok(items);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddDto dto)
        {
            var existing = await _db.CartItems.SingleOrDefaultAsync(c => c.UserId == UserId && c.ProductId == dto.ProductId);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                _db.CartItems.Add(new CartItem { UserId = UserId, ProductId = dto.ProductId, Quantity = dto.Quantity });
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            var items = await _db.CartItems.Where(c => c.UserId == UserId).ToListAsync();
            if (items.Count == 0) return BadRequest(new { error = "Cart is empty" });

            decimal total = 0m;
            var invoice = new Invoice { UserId = UserId, InvoiceNumber = "INV-" + Guid.NewGuid().ToString().Substring(0,8), Status = "Paid", BillingAddress = dto.BillingAddress };
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();

            foreach (var ci in items)
            {
                var product = await _db.Products.FindAsync(ci.ProductId);
                if (product == null) continue;

                var line = new InvoiceItem { InvoiceId = invoice.Id, ProductId = product.Id, Quantity = ci.Quantity, UnitPrice = product.Price, LineTotal = product.Price * ci.Quantity };
                total += line.LineTotal;
                _db.InvoiceItems.Add(line);
            }
            invoice.Total = total;
            invoice.PaidAt = DateTime.UtcNow;

            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();
            return Ok(new { invoiceId = invoice.Id, total });
        }
    }

    public class AddDto { public Guid ProductId { get; set; } public int Quantity { get; set; } }
    public class CheckoutDto { public string BillingAddress { get; set; } = string.Empty; }
}
