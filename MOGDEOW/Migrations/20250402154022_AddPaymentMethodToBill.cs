using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOGDEOW.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodToBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Bills",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "Unknown");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Bills");
        }
    }
}
