using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBookStoreMVC.Migrations
{
    /// <inheritdoc />
    public partial class Updateonaddressentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Addresses",
                newName: "Region");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Addresses",
                newName: "DeliveryAddress");

            migrationBuilder.AddColumn<string>(
                name: "AddittionalPhoneNumber",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddittionalPhoneNumber",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "Region",
                table: "Addresses",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "DeliveryAddress",
                table: "Addresses",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
