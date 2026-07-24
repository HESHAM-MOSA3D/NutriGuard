using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NutriGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FoodPreferences_HealthProfileId",
                table: "FoodPreferences");

            migrationBuilder.DropColumn(
                name: "FoodName",
                table: "FoodPreferences");

            migrationBuilder.AddColumn<int>(
                name: "FoodId",
                table: "FoodPreferences",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ArabicName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodPreferences_FoodId",
                table: "FoodPreferences",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPreferences_HealthProfileId_FoodId",
                table: "FoodPreferences",
                columns: new[] { "HealthProfileId", "FoodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Foods_Name",
                table: "Foods",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FoodPreferences_Foods_FoodId",
                table: "FoodPreferences",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodPreferences_Foods_FoodId",
                table: "FoodPreferences");

            migrationBuilder.DropTable(
                name: "Foods");

            migrationBuilder.DropIndex(
                name: "IX_FoodPreferences_FoodId",
                table: "FoodPreferences");

            migrationBuilder.DropIndex(
                name: "IX_FoodPreferences_HealthProfileId_FoodId",
                table: "FoodPreferences");

            migrationBuilder.DropColumn(
                name: "FoodId",
                table: "FoodPreferences");

            migrationBuilder.AddColumn<string>(
                name: "FoodName",
                table: "FoodPreferences",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPreferences_HealthProfileId",
                table: "FoodPreferences",
                column: "HealthProfileId");
        }
    }
}
