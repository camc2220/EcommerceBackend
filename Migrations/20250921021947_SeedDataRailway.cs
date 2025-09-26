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
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(@"
                    CREATE TABLE IF NOT EXISTS ""Users"" (
                        ""Id"" uuid PRIMARY KEY,
                        ""Email"" text NOT NULL,
                        ""NormalizedEmail"" text NOT NULL DEFAULT '',
                        ""PasswordHash"" text NOT NULL,
                        ""FullName"" text NULL,
                        ""Role"" text NOT NULL,
                        ""CreatedAt"" timestamptz NOT NULL
                    );
                ");
                migrationBuilder.Sql(@"
                    CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Users_NormalizedEmail""
                    ON ""Users"" (""NormalizedEmail"");
                ");

                migrationBuilder.Sql(@"
                    CREATE TABLE IF NOT EXISTS ""Products"" (
                        ""Id"" uuid PRIMARY KEY,
                        ""Name"" text NOT NULL,
                        ""Description"" text NULL,
                        ""Price"" numeric NOT NULL,
                        ""Stock"" integer NOT NULL,
                        ""Category"" text NULL,
                        ""ImageUrl"" text NULL,
                        ""CreatedAt"" timestamptz NOT NULL
                    );
                ");

                migrationBuilder.Sql(@"
                    CREATE TABLE IF NOT EXISTS ""Invoices"" (
                        ""Id"" uuid PRIMARY KEY,
                        ""UserId"" uuid NOT NULL,
                        ""InvoiceNumber"" text NOT NULL,
                        ""Total"" numeric NOT NULL,
                        ""Tax"" numeric NOT NULL,
                        ""Status"" text NOT NULL,
                        ""CreatedAt"" timestamptz NOT NULL,
                        ""PaidAt"" timestamptz NULL,
                        ""BillingAddress"" text NULL
                    );
                ");

                migrationBuilder.Sql(@"
                    CREATE TABLE IF NOT EXISTS ""Payments"" (
                        ""Id"" uuid PRIMARY KEY,
                        ""InvoiceId"" uuid NOT NULL,
                        ""Provider"" text NULL,
                        ""ProviderPaymentId"" text NULL,
                        ""Amount"" numeric NOT NULL,
                        ""Status"" text NOT NULL,
                        ""CreatedAt"" timestamptz NOT NULL
                    );
                ");

                migrationBuilder.Sql(@"
                    CREATE TABLE IF NOT EXISTS ""CartItems"" (
                        ""Id"" uuid PRIMARY KEY,
                        ""UserId"" uuid NOT NULL,
                        ""ProductId"" uuid NOT NULL,
                        ""Quantity"" integer NOT NULL,
                        ""CreatedAt"" timestamptz NOT NULL
                    );
                ");

                migrationBuilder.Sql(@"
                    CREATE TABLE IF NOT EXISTS ""InvoiceItems"" (
                        ""Id"" uuid PRIMARY KEY,
                        ""InvoiceId"" uuid NOT NULL,
                        ""ProductId"" uuid NOT NULL,
                        ""Quantity"" integer NOT NULL,
                        ""UnitPrice"" numeric NOT NULL,
                        ""LineTotal"" numeric NOT NULL,
                        CONSTRAINT ""FK_InvoiceItems_Invoices_InvoiceId"" 
                            FOREIGN KEY (""InvoiceId"") REFERENCES ""Invoices"" (""Id"") ON DELETE CASCADE
                    );
                ");

                migrationBuilder.Sql(@"
                    CREATE INDEX IF NOT EXISTS ""IX_InvoiceItems_InvoiceId"" 
                    ON ""InvoiceItems"" (""InvoiceId"");
                ");
            }

            var adminCreatedAt = new DateTime(2025, 9, 21, 2, 19, 46, 202, DateTimeKind.Utc).AddTicks(1762);
            var userCreatedAt = new DateTime(2025, 9, 21, 2, 19, 46, 468, DateTimeKind.Utc).AddTicks(7329);
            var seedBase = new DateTime(2025, 9, 21, 2, 19, 46, 692, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "NormalizedEmail", "PasswordHash", "Role" },
                values: new object[,]
                {
                    {
                        new Guid("89b8105d-38e1-4849-abe3-d7c20b99d8a4"),
                        adminCreatedAt,
                        "admin@ecommerce.com",
                        "Admin",
                        "admin@ecommerce.com",
                        "$2a$12$VCJVrEjQn17n3Vdd4QXdyOjRJr3BZ1M70Y/JlDlh.wur8H.nZwYYO.",
                        "admin"
                    },
                    {
                        new Guid("d60ec960-bc53-424a-9128-5275fbd4969f"),
                        userCreatedAt,
                        "user@ecommerce.com",
                        "Usuario Test",
                        "user@ecommerce.com",
                        "$2a$12$MwOFPEsAFNeID.N9x139jObpJuIlrXzIUMY/ASgGrxUDcLh80CT1W",
                        "user"
                    }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    {
                        new Guid("488e5e0f-e6eb-44a3-a587-9c3e5f6c5e7b"),
                        "Electrónica",
                        seedBase.AddTicks(4094),
                        "Laptop potente para gaming",
                        string.Empty,
                        "Laptop Gamer",
                        1500m,
                        10
                    },
                    {
                        new Guid("0dd2df11-f26b-4b0a-bd2a-eb9f1c1f94f6"),
                        "Accesorios",
                        seedBase.AddTicks(4106),
                        "Mouse Bluetooth ergonómico",
                        string.Empty,
                        "Mouse Inalámbrico",
                        35m,
                        50
                    }
                });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "Id", "CreatedAt", "ProductId", "Quantity", "UserId" },
                values: new object[]
                {
                    new Guid("1e8537ac-5a83-4f32-9ba5-248358dae55a"),
                    seedBase.AddTicks(4192),
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
                    seedBase.AddTicks(4258),
                    "INV-001",
                    seedBase.AddTicks(4267),
                    "Paid",
                    0m,
                    70m,
                    new Guid("d60ec960-bc53-424a-9128-5275fbd4969f")
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

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "CreatedAt", "InvoiceId", "Provider", "ProviderPaymentId", "Status" },
                values: new object[]
                {
                    new Guid("cb361e92-3347-46f6-8568-8c1671dfbd8d"),
                    70m,
                    seedBase.AddTicks(4383),
                    new Guid("d30b0c2c-d81c-4268-b9ff-1fcaebdb4006"),
                    "Stripe",
                    "pay_001",
                    "Completed"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_InvoiceItems_InvoiceId"";");
                migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Users_NormalizedEmail"";");
                migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""CartItems"" CASCADE;");
                migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""InvoiceItems"" CASCADE;");
                migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""Payments"" CASCADE;");
                migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""Products"" CASCADE;");
                migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""Users"" CASCADE;");
                migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""Invoices"" CASCADE;");
            }
            else
            {
                migrationBuilder.DropTable("CartItems");
                migrationBuilder.DropTable("InvoiceItems");
                migrationBuilder.DropTable("Payments");
                migrationBuilder.DropTable("Products");
                migrationBuilder.DropTable("Users");
                migrationBuilder.DropTable("Invoices");
            }
        }
    }
}
