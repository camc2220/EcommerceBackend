-- Initial schema for EcommerceBackend (PostgreSQL)
CREATE TABLE IF NOT EXISTS "Users" (
  "Id" uuid PRIMARY KEY,
  "Email" varchar(256) NOT NULL UNIQUE,
  "NormalizedEmail" varchar(256) NOT NULL DEFAULT '',
  "PasswordHash" varchar(512) NOT NULL,
  "FullName" varchar(200),
  "Role" varchar(50),
  "CreatedAt" timestamptz DEFAULT now()
);
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_NormalizedEmail" ON "Users" ("NormalizedEmail");
CREATE TABLE IF NOT EXISTS "Products" (
  "Id" uuid PRIMARY KEY,
  "Name" varchar(200) NOT NULL,
  "Description" text,
  "Price" numeric(18,2) NOT NULL,
  "Stock" integer NOT NULL DEFAULT 0,
  "Category" varchar(100),
  "ImageUrl" varchar(1000),
  "CreatedAt" timestamptz DEFAULT now()
);
CREATE TABLE IF NOT EXISTS "CartItems" (
  "Id" uuid PRIMARY KEY,
  "UserId" uuid REFERENCES "Users"("Id") ON DELETE CASCADE,
  "ProductId" uuid REFERENCES "Products"("Id") ON DELETE CASCADE,
  "Quantity" integer NOT NULL,
  "CreatedAt" timestamptz DEFAULT now()
);
CREATE TABLE IF NOT EXISTS "Invoices" (
  "Id" uuid PRIMARY KEY,
  "UserId" uuid REFERENCES "Users"("Id"),
  "InvoiceNumber" varchar(100) NOT NULL,
  "Total" numeric(18,2) NOT NULL,
  "Tax" numeric(18,2) DEFAULT 0,
  "Status" varchar(50),
  "CreatedAt" timestamptz DEFAULT now(),
  "PaidAt" timestamptz,
  "BillingAddress" text
);
CREATE TABLE IF NOT EXISTS "InvoiceItems" (
  "Id" uuid PRIMARY KEY,
  "InvoiceId" uuid REFERENCES "Invoices"("Id") ON DELETE CASCADE,
  "ProductId" uuid REFERENCES "Products"("Id"),
  "Quantity" integer NOT NULL,
  "UnitPrice" numeric(18,2) NOT NULL,
  "LineTotal" numeric(18,2) NOT NULL
);
CREATE INDEX IF NOT EXISTS "IX_InvoiceItems_InvoiceId" ON "InvoiceItems" ("InvoiceId");
CREATE TABLE IF NOT EXISTS "Payments" (
  "Id" uuid PRIMARY KEY,
  "InvoiceId" uuid REFERENCES "Invoices"("Id"),
  "Provider" varchar(100),
  "ProviderPaymentId" varchar(200),
  "Amount" numeric(18,2),
  "Status" varchar(50),
  "CreatedAt" timestamptz DEFAULT now()
);

INSERT INTO "Users" ("Id", "Email", "NormalizedEmail", "PasswordHash", "FullName", "Role", "CreatedAt") VALUES
  ('89b8105d-38e1-4849-abe3-d7c20b99d8a4', 'admin@ecommerce.com', 'admin@ecommerce.com', '$2a$12$VCJVrEjQn17n3Vdd4QXdyOjRJr3BZ1M70Y/JlDlh.wur8H.nZwYYO.', 'Admin', 'admin', '2025-09-21 02:19:46.202176+00'),
  ('d60ec960-bc53-424a-9128-5275fbd4969f', 'user@ecommerce.com', 'user@ecommerce.com', '$2a$12$MwOFPEsAFNeID.N9x139jObpJuIlrXzIUMY/ASgGrxUDcLh80CT1W', 'Usuario Test', 'user', '2025-09-21 02:19:46.468733+00')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Products" ("Id", "Name", "Description", "Price", "Stock", "Category", "ImageUrl", "CreatedAt") VALUES
  ('488e5e0f-e6eb-44a3-a587-9c3e5f6c5e7b', 'Laptop Gamer', 'Laptop potente para gaming', 1500, 10, 'Electrónica', '', '2025-09-21 02:19:46.692409+00'),
  ('0dd2df11-f26b-4b0a-bd2a-eb9f1c1f94f6', 'Mouse Inalámbrico', 'Mouse Bluetooth ergonómico', 35, 50, 'Accesorios', '', '2025-09-21 02:19:46.692411+00')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "CartItems" ("Id", "UserId", "ProductId", "Quantity", "CreatedAt") VALUES
  ('1e8537ac-5a83-4f32-9ba5-248358dae55a', 'd60ec960-bc53-424a-9128-5275fbd4969f', '0dd2df11-f26b-4b0a-bd2a-eb9f1c1f94f6', 2, '2025-09-21 02:19:46.692419+00')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Invoices" ("Id", "UserId", "InvoiceNumber", "Total", "Tax", "Status", "CreatedAt", "PaidAt", "BillingAddress") VALUES
  ('d30b0c2c-d81c-4268-b9ff-1fcaebdb4006', 'd60ec960-bc53-424a-9128-5275fbd4969f', 'INV-001', 70, 0, 'Paid', '2025-09-21 02:19:46.692426+00', '2025-09-21 02:19:46.692427+00', 'Calle Falsa 123')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "InvoiceItems" ("Id", "InvoiceId", "ProductId", "Quantity", "UnitPrice", "LineTotal") VALUES
  ('c8633a68-aea5-4904-a9b5-5a15de248cc4', 'd30b0c2c-d81c-4268-b9ff-1fcaebdb4006', '0dd2df11-f26b-4b0a-bd2a-eb9f1c1f94f6', 2, 35, 70)
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Payments" ("Id", "InvoiceId", "Provider", "ProviderPaymentId", "Amount", "Status", "CreatedAt") VALUES
  ('cb361e92-3347-46f6-8568-8c1671dfbd8d', 'd30b0c2c-d81c-4268-b9ff-1fcaebdb4006', 'Stripe', 'pay_001', 70, 'Completed', '2025-09-21 02:19:46.692438+00')
ON CONFLICT ("Id") DO NOTHING;
