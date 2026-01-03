using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOGDEOW.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiverInfoToBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "Bills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverPhone",
                table: "Bills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Bills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "ReceiverPhone",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Bills");
        }
    }
}
