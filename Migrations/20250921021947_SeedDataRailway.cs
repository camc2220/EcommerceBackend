using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcommerceBackend.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataRailway : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BillingAddress = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    ProviderPaymentId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "Id", "CreatedAt", "ProductId", "Quantity", "UserId" },
                values: new object[]
                {
                    new Guid("1e8537ac-5a83-4f32-9ba5-248358dae55a"),
                    new DateTime(2025, 9, 21, 2, 19, 46, 692, DateTimeKind.Utc).AddTicks(4192),
                    new Guid("0dd2df11-f26b-4b0a-bd2a-eb9f1c1f94f6"),
                    2,
                    new Guid("d60ec960-bc53-424a-9128-5275fbd4969f")
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "Id", "BillingAddress", "CreatedAt", "InvoiceNumber", "PaidAt", "Status", "Tax", "Total", "UserId" },
                values: new object[]
                {
                    new Guid("d30b0c2c-d81c-4268-b9ff-1fcaebdb4006"),
                    "Calle Falsa 123",
                    new DateTime(2025, 9, 21, 2, 19, 46, 692, DateTimeKind.Utc).AddTicks(4258),
                    "INV-001",
                    new DateTime(2025, 9, 21, 2, 19, 46, 692, DateTimeKind.Utc).AddTicks(4267),
                    "Paid",
                    0m,
                    70m,
                    new Guid("d60ec960-bc53-424a-9128-5275fbd4969f")
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "CreatedAt", "InvoiceId", "Provider", "ProviderPaymentId", "Status" },
                values: new object[]
                {
                    new Guid("cb361e92-3347-46f6-8568-8c1671dfbd8d"),
                    70m,
                    new DateTime(2025, 9, 21, 2, 19, 46, 692, DateTimeKind.Utc).AddTicks(4383),
                    new Guid("d30b0c2c-d81c-4268-b9ff-1fcaebdb4006"),
                    "Stripe",
                    "pay_001",
                    "Completed"
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    {
                        new Guid("0dd2df11-f26b-4b0a-bd2a-eb9f1c1f94f6"),
                        "Accesorios",
                        new DateTime(2025, 9, 21, 2, 19, 46, 692, DateTimeKind.Utc).AddTicks(4106),
                        "Mouse Bluetooth ergonómico",
                        string.Empty,
                        "Mouse Inalámbrico",
                        35m,
                        50
                    },
                    {
                        new Guid("488e5e0f-e6eb-44a3-a587-9c3e5f6c5e7b"),
                        "Electrónica",
                        new DateTime(2025, 9, 21, 2, 19, 46, 692, DateTimeKind.Utc).AddTicks(4094),
                        "Laptop potente para gaming",
                        string.Empty,
                        "Laptop Gamer",
                        1500m,
                        10
                    }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "PasswordHash", "Role" },
                values: new object[,]
                {
                    {
                        new Guid("89b8105d-38e1-4849-abe3-d7c20b99d8a4"),
                        new DateTime(2025, 9, 21, 2, 19, 46, 202, DateTimeKind.Utc).AddTicks(1762),
                        "admin@ecommerce.com",
                        "Admin",
                        "$2a$11$a4XTN3Qs2Pls1KQrRWDlNOEEOLNW5Tn9lUHGYAl0wlgg5cYmHT4f.",
                        "admin"
                    },
                    {
                        new Guid("d60ec960-bc53-424a-9128-5275fbd4969f"),
                        new DateTime(2025, 9, 21, 2, 19, 46, 468, DateTimeKind.Utc).AddTicks(7329),
                        "user@ecommerce.com",
                        "Usuario Test",
                        "$2a$11$SBZkm5ho6nodxsu1bjQYKeV5rUlWPqwLMxNYvvSZtEkJRD4uHrSjm",
                        "user"
                    }
                });

            migrationBuilder.InsertData(
                table: "InvoiceItems",
                columns: new[] { "Id", "InvoiceId", "LineTotal", "ProductId", "Quantity", "UnitPrice" },
                values: new object[]
                {
                    new Guid("c8633a68-aea5-4904-a9b5-5a15de248cc4"),
                    new Guid("d30b0c2c-d81c-4268-b9ff-1fcaebdb4006"),
                    70m,
                    new Guid("0dd2df11-f26b-4b0a-bd2a-eb9f1c1f94f6"),
                    2,
                    35m
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "InvoiceItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Invoices");
        }
    }
}
