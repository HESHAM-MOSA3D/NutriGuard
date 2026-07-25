using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NutriGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FoodPreferences_HealthProfileId_FoodId",
                table: "FoodPreferences");

            migrationBuilder.AlterColumn<string>(
                name: "ArabicName",
                table: "Foods",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ash",
                table: "Foods",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Calcium",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Carbohydrate",
                table: "Foods",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Copper",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Energy",
                table: "Foods",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Fat",
                table: "Foods",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Fiber",
                table: "Foods",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Iron",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Magnesium",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Phosphorus",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Potassium",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Protein",
                table: "Foods",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RefusePercentage",
                table: "Foods",
                type: "numeric(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Riboflavin",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Sodium",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Thiamin",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminA",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminC",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Water",
                table: "Foods",
                type: "numeric(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Zinc",
                table: "Foods",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
               name: "CreatedAt",
               table: "FoodPreferences",
               type: "timestamp with time zone",
               nullable: false,
               defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateTable(
                name: "FoodAliases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FoodId = table.Column<int>(type: "integer", nullable: false),
                    Alias = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodAliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodAliases_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodPreferences_HealthProfileId_FoodId_PreferenceType",
                table: "FoodPreferences",
                columns: new[] { "HealthProfileId", "FoodId", "PreferenceType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodAliases_Alias",
                table: "FoodAliases",
                column: "Alias");

            migrationBuilder.CreateIndex(
                name: "IX_FoodAliases_FoodId",
                table: "FoodAliases",
                column: "FoodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodAliases");

            migrationBuilder.DropIndex(
                name: "IX_FoodPreferences_HealthProfileId_FoodId_PreferenceType",
                table: "FoodPreferences");

            migrationBuilder.DropColumn(
                name: "Ash",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Calcium",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Carbohydrate",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Copper",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Energy",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Fat",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Fiber",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Iron",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Magnesium",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Phosphorus",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Potassium",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Protein",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "RefusePercentage",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Riboflavin",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Sodium",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Thiamin",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "VitaminA",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "VitaminC",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Water",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Zinc",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FoodPreferences");

            migrationBuilder.AlterColumn<string>(
                name: "ArabicName",
                table: "Foods",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodPreferences_HealthProfileId_FoodId",
                table: "FoodPreferences",
                columns: new[] { "HealthProfileId", "FoodId" },
                unique: true);
        }
    }
}
