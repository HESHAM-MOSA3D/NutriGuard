using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFoodAliasDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FoodAliases_Alias",
                table: "FoodAliases");

            migrationBuilder.DropIndex(
                name: "IX_FoodAliases_FoodId",
                table: "FoodAliases");

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "FoodAliases",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FoodAliases_Alias_Language",
                table: "FoodAliases",
                columns: new[] { "Alias", "Language" });

            migrationBuilder.CreateIndex(
                name: "IX_FoodAliases_FoodId_Alias_Language",
                table: "FoodAliases",
                columns: new[] { "FoodId", "Alias", "Language" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FoodAliases_Alias_Language",
                table: "FoodAliases");

            migrationBuilder.DropIndex(
                name: "IX_FoodAliases_FoodId_Alias_Language",
                table: "FoodAliases");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "FoodAliases");

            migrationBuilder.CreateIndex(
                name: "IX_FoodAliases_Alias",
                table: "FoodAliases",
                column: "Alias");

            migrationBuilder.CreateIndex(
                name: "IX_FoodAliases_FoodId",
                table: "FoodAliases",
                column: "FoodId");
        }
    }
}
