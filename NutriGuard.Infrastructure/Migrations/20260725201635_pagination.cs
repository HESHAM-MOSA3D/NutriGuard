using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class pagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Foods_Name",
                table: "Foods",
                newName: "ix_foods_name_unique");

            migrationBuilder.RenameIndex(
                name: "IX_Foods_FoodCategoryId",
                table: "Foods",
                newName: "idx_foods_categoryid");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateIndex(
                name: "idx_foods_name_trgm",
                table: "Foods",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_foodaliases_alias_trgm",
                table: "FoodAliases",
                column: "Alias")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_foods_name_trgm",
                table: "Foods");

            migrationBuilder.DropIndex(
                name: "idx_foodaliases_alias_trgm",
                table: "FoodAliases");

            migrationBuilder.RenameIndex(
                name: "ix_foods_name_unique",
                table: "Foods",
                newName: "IX_Foods_Name");

            migrationBuilder.RenameIndex(
                name: "idx_foods_categoryid",
                table: "Foods",
                newName: "IX_Foods_FoodCategoryId");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");
        }
    }
}
