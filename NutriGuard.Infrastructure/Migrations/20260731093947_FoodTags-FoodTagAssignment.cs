using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NutriGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FoodTagsFoodTagAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodTagAssignments",
                columns: table => new
                {
                    FoodId = table.Column<int>(type: "integer", nullable: false),
                    FoodTagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodTagAssignments", x => new { x.FoodId, x.FoodTagId });
                    table.ForeignKey(
                        name: "FK_FoodTagAssignments_FoodTags_FoodTagId",
                        column: x => x.FoodTagId,
                        principalTable: "FoodTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodTagAssignments_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodTagAssignments_FoodTagId",
                table: "FoodTagAssignments",
                column: "FoodTagId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodTags_Name",
                table: "FoodTags",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodTagAssignments");

            migrationBuilder.DropTable(
                name: "FoodTags");
        }
    }
}
