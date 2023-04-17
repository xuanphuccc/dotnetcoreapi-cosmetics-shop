using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_api_cosmetics_shop.Migrations
{
    public partial class isDisplayaddresspaymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_PaymentMethods_PaymentMethodId",
                table: "ShopOrders");

            migrationBuilder.AddColumn<bool>(
                name: "IsDisplay",
                table: "PaymentMethods",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisplay",
                table: "Addresses",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_PaymentMethods_PaymentMethodId",
                table: "ShopOrders",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_PaymentMethods_PaymentMethodId",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "IsDisplay",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "IsDisplay",
                table: "Addresses");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_PaymentMethods_PaymentMethodId",
                table: "ShopOrders",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
