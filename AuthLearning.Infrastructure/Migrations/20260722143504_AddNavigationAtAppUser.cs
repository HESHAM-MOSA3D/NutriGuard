using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriGuard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationAtAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HealthProfileId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_HealthProfileId",
                table: "AspNetUsers",
                column: "HealthProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_HealthProfiles_HealthProfileId",
                table: "AspNetUsers",
                column: "HealthProfileId",
                principalTable: "HealthProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_HealthProfiles_HealthProfileId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_HealthProfileId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HealthProfileId",
                table: "AspNetUsers");
        }
    }
}
