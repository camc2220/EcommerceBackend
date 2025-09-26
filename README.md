Ecommerce backend + frontend ready for Railway. See RAILWAY_INSTRUCTIONS.txt for deployment steps.

## Database bootstrap

The project now depends on the `NormalizedEmail` column for login and includes deterministic seed data for users, products, invoices, and payments. On clean environments you can either run the EF Core migrations or execute `initial_schema.sql` before starting the API.

If you already provisioned a PostgreSQL database with an older schema, run the following once before redeploying:

```sql
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "NormalizedEmail" text NOT NULL DEFAULT '';
UPDATE "Users" SET "NormalizedEmail" = lower("Email") WHERE coalesce("NormalizedEmail", '') = '';
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_NormalizedEmail" ON "Users" ("NormalizedEmail");
```

Afterwards, re-run the migrations (`dotnet ef database update` locally or restart the Railway service) so that the deterministic seed records are inserted and GET endpoints stop returning 500 due to missing tables or rows.
