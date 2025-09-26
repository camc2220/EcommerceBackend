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
                        ""PasswordHash"" text NOT NULL,
                        ""FullName"" text NULL,
                        ""Role"" text NOT NULL,
                        ""CreatedAt"" timestamptz NOT NULL
                    );
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

            // InsertData (igual que ya lo tienes después)...
            var seedTimestamp = new DateTime(2025, 9, 21, 2, 19, 46, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "PasswordHash", "Role" },
                values: new object[,]
                {
                    {
                        new Guid("89b8105d-38e1-4849-abe3-d7c20b99d8a4"),
                        seedTimestamp,
                        "admin@ecommerce.com",
                        "Admin",
                        "$2a$11$a4XTN3Qs2Pls1KQrRWDlNOEEOLNW5Tn9lUHGYAl0wlgg5cYmHT4f.",
                        "admin"
                    },
                    {
                        new Guid("d60ec960-bc53-424a-9128-5275fbd4969f"),
                        seedTimestamp,
                        "user@ecommerce.com",
                        "Usuario Test",
                        "$2a$11$SBZkm5ho6nodxsu1bjQYKeV5rUlWPqwLMxNYvvSZtEkJRD4uHrSjm",
                        "user"
                    }
                });

            // ... y el resto de Inserts igual
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_InvoiceItems_InvoiceId"";");
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
