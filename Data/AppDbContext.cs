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

            // --- Usuarios ---
            var adminId = Guid.NewGuid();
            var userId = Guid.NewGuid();

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
            var product1Id = Guid.NewGuid();
            var product2Id = Guid.NewGuid();

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

            // --- Carrito de usuario ---
            var cartItemId = Guid.NewGuid();

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
            var invoiceId = Guid.NewGuid();

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
            var invoiceItemId = Guid.NewGuid();

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
            var paymentId = Guid.NewGuid();
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
