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

        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var subject = User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return !string.IsNullOrWhiteSpace(subject) && Guid.TryParse(subject, out userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var items = await _db.CartItems
                .Where(c => c.UserId == userId)
                .Join(
                    _db.Products,
                    cart => cart.ProductId,
                    product => product.Id,
                    (cart, product) => new CartItemResponseDto
                    {
                        CartItemId = cart.Id,
                        ProductId = cart.ProductId,
                        Quantity = cart.Quantity,
                        CreatedAt = cart.CreatedAt,
                        Product = new ProductSummaryDto
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            Stock = product.Stock,
                            Category = product.Category,
                            ImageUrl = product.ImageUrl
                        }
                    })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddDto dto)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (dto.Quantity <= 0)
                return BadRequest(new { error = "Quantity must be greater than zero." });

            var existing = await _db.CartItems.SingleOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                _db.CartItems.Add(new CartItem { UserId = userId, ProductId = dto.ProductId, Quantity = dto.Quantity });
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityDto dto)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (dto.Quantity <= 0)
                return BadRequest(new { error = "Quantity must be greater than zero." });

            var item = await _db.CartItems.SingleOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);
            if (item == null)
                return NotFound();

            item.Quantity = dto.Quantity;
            await _db.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPatch("items/{cartItemId:guid}/quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity(Guid cartItemId, [FromBody] UpdateCartItemQuantityRequest dto)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (dto == null)
                return BadRequest(new { error = "Request body is required." });

            if (dto.Quantity <= 0)
                return BadRequest(new { error = "Quantity must be greater than zero." });

            var item = await _db.CartItems.SingleOrDefaultAsync(c => c.UserId == userId && c.Id == cartItemId);
            if (item == null)
                return NotFound();

            item.Quantity = dto.Quantity;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                item.Id,
                item.ProductId,
                item.Quantity
            });
        }

        [HttpDelete("items/{cartItemId:guid}")]
        public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var item = await _db.CartItems.SingleOrDefaultAsync(c => c.UserId == userId && c.Id == cartItemId);
            if (item == null)
                return NotFound();

            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var items = await _db.CartItems.Where(c => c.UserId == userId).ToListAsync();
            if (items.Count == 0) return BadRequest(new { error = "Cart is empty" });

            decimal total = 0m;
            var invoice = new Invoice { UserId = userId, InvoiceNumber = "INV-" + Guid.NewGuid().ToString().Substring(0,8), Status = "Paid", BillingAddress = dto.BillingAddress };
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
    public class UpdateQuantityDto { public Guid ProductId { get; set; } public int Quantity { get; set; } }
    public class CheckoutDto { public string BillingAddress { get; set; } = string.Empty; }
    public class UpdateCartItemQuantityRequest { public int Quantity { get; set; } }
    public class CartItemResponseDto
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProductSummaryDto Product { get; set; }
    }

    public class ProductSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
    }
}
