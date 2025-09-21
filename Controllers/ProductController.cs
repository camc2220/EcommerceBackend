using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceBackend.Data;
using EcommerceBackend.Models;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _db.Products.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [Authorize(Roles="admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product p)
        {
            p.Id = Guid.NewGuid();
            _db.Products.Add(p);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = p.Id }, p);
        }

        [Authorize(Roles="admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Product update)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();

            p.Name = update.Name;
            p.Description = update.Description;
            p.Price = update.Price;
            p.Stock = update.Stock;
            p.Category = update.Category;
            p.ImageUrl = update.ImageUrl;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles="admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();

            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
