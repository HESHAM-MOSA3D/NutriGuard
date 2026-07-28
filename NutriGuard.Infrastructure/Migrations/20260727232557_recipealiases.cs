using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NutriGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class recipealiases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecipeAliases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipeId = table.Column<int>(type: "integer", nullable: false),
                    Alias = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeAliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeAliases_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeAliases_RecipeId_Alias",
                table: "RecipeAliases",
                columns: new[] { "RecipeId", "Alias" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeAliases");
        }
    }
}
