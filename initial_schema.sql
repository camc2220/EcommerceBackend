-- Initial schema for EcommerceBackend (PostgreSQL)
CREATE TABLE IF NOT EXISTS "Users" (
  "Id" uuid PRIMARY KEY,
  "Email" varchar(256) NOT NULL UNIQUE,
  "PasswordHash" varchar(512) NOT NULL,
  "FullName" varchar(200),
  "Role" varchar(50),
  "CreatedAt" timestamptz DEFAULT now()
);
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
CREATE TABLE IF NOT EXISTS "Payments" (
  "Id" uuid PRIMARY KEY,
  "InvoiceId" uuid REFERENCES "Invoices"("Id"),
  "Provider" varchar(100),
  "ProviderPaymentId" varchar(200),
  "Amount" numeric(18,2),
  "Status" varchar(50),
  "CreatedAt" timestamptz DEFAULT now()
);
