using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_api_cosmetics_shop.Migrations
{
    public partial class fixpayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardholderName",
                table: "PaymentMethods",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "PaymentMethods",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityCode",
                table: "PaymentMethods",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardholderName",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "SecurityCode",
                table: "PaymentMethods");
        }
    }
}
