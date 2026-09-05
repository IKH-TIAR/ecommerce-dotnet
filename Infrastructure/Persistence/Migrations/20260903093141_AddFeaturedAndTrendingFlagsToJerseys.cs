using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturedAndTrendingFlagsToJerseys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Jerseys",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTrending",
                table: "Jerseys",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Jerseys_IsFeatured",
                table: "Jerseys",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_Jerseys_IsTrending",
                table: "Jerseys",
                column: "IsTrending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jerseys_IsFeatured",
                table: "Jerseys");

            migrationBuilder.DropIndex(
                name: "IX_Jerseys_IsTrending",
                table: "Jerseys");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Jerseys");

            migrationBuilder.DropColumn(
                name: "IsTrending",
                table: "Jerseys");
        }
    }
}
