using System;
using Microsoft.EntityFrameworkCore;
using EcommerceBackend.Models;

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

        public override int SaveChanges()
        {
            NormalizeUserEmails();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            NormalizeUserEmails();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Identificadores determinísticos para coincidir con las migraciones ---
            var adminId = Guid.Parse("89b8105d-38e1-4849-abe3-d7c20b99d8a4");
            var userId = Guid.Parse("d60ec960-bc53-424a-9128-5275fbd4969f");
            var product1Id = Guid.Parse("488e5e0f-e6eb-44a3-a587-9c3e5f6c5e7b");
            var product2Id = Guid.Parse("0dd2df11-f26b-4b0a-bd2a-eb9f1c1f94f6");
            var cartItemId = Guid.Parse("1e8537ac-5a83-4f32-9ba5-248358dae55a");
            var invoiceId = Guid.Parse("d30b0c2c-d81c-4268-b9ff-1fcaebdb4006");
            var invoiceItemId = Guid.Parse("c8633a68-aea5-4904-a9b5-5a15de248cc4");
            var paymentId = Guid.Parse("cb361e92-3347-46f6-8568-8c1671dfbd8d");

            var userCreatedAt = new DateTime(2025, 9, 21, 2, 19, 46, 202, DateTimeKind.Utc).AddTicks(1762);
            var customerCreatedAt = new DateTime(2025, 9, 21, 2, 19, 46, 468, DateTimeKind.Utc).AddTicks(7329);
            var productCreatedAt = new DateTime(2025, 9, 21, 2, 19, 46, 692, DateTimeKind.Utc);

            // --- Usuarios ---
            modelBuilder.Entity<User>()
                .HasIndex(u => u.NormalizedEmail)
                .IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    Email = "admin@ecommerce.com",
                    NormalizedEmail = "admin@ecommerce.com",
                    FullName = "Admin",
                    PasswordHash = "$2a$12$VCJVrEjQn17n3Vdd4QXdyOjRJr3BZ1M70Y/JlDlh.wur8H.nZwYYO.",
                    Role = "admin",
                    CreatedAt = userCreatedAt
                },
                new User
                {
                    Id = userId,
                    Email = "user@ecommerce.com",
                    NormalizedEmail = "user@ecommerce.com",
                    FullName = "Usuario Test",
                    PasswordHash = "$2a$12$MwOFPEsAFNeID.N9x139jObpJuIlrXzIUMY/ASgGrxUDcLh80CT1W",
                    Role = "user",
                    CreatedAt = customerCreatedAt
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
                    ImageUrl = "",
                    CreatedAt = productCreatedAt.AddTicks(4094)
                },
                new Product
                {
                    Id = product2Id,
                    Name = "Mouse Inalámbrico",
                    Description = "Mouse Bluetooth ergonómico",
                    Price = 35m,
                    Stock = 50,
                    Category = "Accesorios",
                    ImageUrl = "",
                    CreatedAt = productCreatedAt.AddTicks(4106)
                }
            );

            // --- Carrito ---
            modelBuilder.Entity<CartItem>().HasData(
                new CartItem
                {
                    Id = cartItemId,
                    UserId = userId,
                    ProductId = product2Id,
                    Quantity = 2,
                    CreatedAt = productCreatedAt.AddTicks(4192)
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
                    Tax = 0m,
                    BillingAddress = "Calle Falsa 123",
                    CreatedAt = productCreatedAt.AddTicks(4258),
                    PaidAt = productCreatedAt.AddTicks(4267)
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
                    CreatedAt = productCreatedAt.AddTicks(4383)
                }
            );
        }

        private void NormalizeUserEmails()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<User>())
            {
                if (entry.State != EntityState.Added && entry.State != EntityState.Modified)
                    continue;

                var email = (entry.Entity.Email ?? string.Empty).Trim();
                entry.Entity.Email = email;
                entry.Entity.NormalizedEmail = email.ToLowerInvariant();

                if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
                {
                    entry.Entity.CreatedAt = now;
                }
            }
        }
    }
}
