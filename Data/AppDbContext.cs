using Microsoft.EntityFrameworkCore;
using EcommerceBackend.Models;
using BCrypt.Net;

namespace EcommerceBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- GUIDs fijos ---
            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var product1Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var product2Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var cartItemId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var invoiceId = Guid.Parse("66666666-6666-6666-6666-666666666666");
            var invoiceItemId = Guid.Parse("77777777-7777-7777-7777-777777777777");
            var paymentId = Guid.Parse("88888888-8888-8888-8888-888888888888");

            // --- Usuarios ---
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    Email = "admin@ecommerce.com",
                    FullName = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = "admin"
                },
                new User
                {
                    Id = userId,
                    Email = "user@ecommerce.com",
                    FullName = "Usuario Test",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                    Role = "user"
                }
            );

            // --- Productos ---
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = product1Id,
                    Name = "Laptop Gamer",
                    Description = "Laptop potente para gaming",
                    Price = 1500m,
                    Stock = 10,
                    Category = "Electrónica",
                    ImageUrl = ""
                },
                new Product
                {
                    Id = product2Id,
                    Name = "Mouse Inalámbrico",
                    Description = "Mouse Bluetooth ergonómico",
                    Price = 35m,
                    Stock = 50,
                    Category = "Accesorios",
                    ImageUrl = ""
                }
            );

            // --- Carrito ---
            modelBuilder.Entity<CartItem>().HasData(
                new CartItem
                {
                    Id = cartItemId,
                    UserId = userId,
                    ProductId = product2Id,
                    Quantity = 2
                }
            );

            // --- Factura ---
            modelBuilder.Entity<Invoice>().HasData(
                new Invoice
                {
                    Id = invoiceId,
                    UserId = userId,
                    InvoiceNumber = "INV-001",
                    Status = "Paid",
                    Total = 70m,
                    BillingAddress = "Calle Falsa 123",
                    PaidAt = DateTime.UtcNow
                }
            );

            // --- Items de factura ---
            modelBuilder.Entity<InvoiceItem>().HasData(
                new InvoiceItem
                {
                    Id = invoiceItemId,
                    InvoiceId = invoiceId,
                    ProductId = product2Id,
                    Quantity = 2,
                    UnitPrice = 35m,
                    LineTotal = 70m
                }
            );

            // --- Pagos ---
            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    Id = paymentId,
                    InvoiceId = invoiceId,
                    Amount = 70m,
                    Provider = "Stripe",
                    ProviderPaymentId = "pay_001",
                    Status = "Completed",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
