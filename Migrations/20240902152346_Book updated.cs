using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBookStoreMVC.Migrations
{
    /// <inheritdoc />
    public partial class Bookupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingQuantity",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalQuantity",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingQuantity",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "TotalQuantity",
                table: "Books");
        }
    }
}
