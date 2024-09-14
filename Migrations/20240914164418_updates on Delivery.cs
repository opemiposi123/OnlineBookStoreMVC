using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBookStoreMVC.Migrations
{
    /// <inheritdoc />
    public partial class updatesonDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TranspotationType",
                table: "Deliveries",
                newName: "TransportationType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransportationType",
                table: "Deliveries",
                newName: "TranspotationType");
        }
    }
}
