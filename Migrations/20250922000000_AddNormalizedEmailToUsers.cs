using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizedEmailToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql("ALTER TABLE \"Users\" ADD COLUMN IF NOT EXISTS \"NormalizedEmail\" text NOT NULL DEFAULT '';");
                migrationBuilder.Sql("UPDATE \"Users\" SET \"NormalizedEmail\" = lower(trim(\"Email\")) WHERE COALESCE(\"NormalizedEmail\", '') = '' OR \"NormalizedEmail\" <> lower(trim(\"Email\"));");
                migrationBuilder.Sql("ALTER TABLE \"Users\" ALTER COLUMN \"NormalizedEmail\" DROP DEFAULT;");
                migrationBuilder.Sql("CREATE UNIQUE INDEX IF NOT EXISTS \"IX_Users_NormalizedEmail\" ON \"Users\" (\"NormalizedEmail\");");
            }
            else
            {
                migrationBuilder.AddColumn<string>(
                    name: "NormalizedEmail",
                    table: "Users",
                    type: "text",
                    nullable: false,
                    defaultValue: "");

                migrationBuilder.Sql("UPDATE \"Users\" SET \"NormalizedEmail\" = lower(trim(\"Email\"));");

                migrationBuilder.CreateIndex(
                    name: "IX_Users_NormalizedEmail",
                    table: "Users",
                    column: "NormalizedEmail",
                    unique: true);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Users_NormalizedEmail\";");
                migrationBuilder.Sql("ALTER TABLE \"Users\" DROP COLUMN IF EXISTS \"NormalizedEmail\";");
            }
            else
            {
                migrationBuilder.DropIndex(
                    name: "IX_Users_NormalizedEmail",
                    table: "Users");

                migrationBuilder.DropColumn(
                    name: "NormalizedEmail",
                    table: "Users");
            }
        }
    }
}
