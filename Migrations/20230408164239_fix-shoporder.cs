using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_api_cosmetics_shop.Migrations
{
    public partial class fixshoporder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountMoney",
                table: "ShopOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCost",
                table: "ShopOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrderStatuses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountMoney",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrderStatuses");
        }
    }
}
