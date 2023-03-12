using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_api_cosmetics_shop.Migrations
{
    public partial class Fix_column_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_OrderStatuses_OderStatusId",
                table: "ShopOrders");

            migrationBuilder.RenameColumn(
                name: "OderStatusId",
                table: "ShopOrders",
                newName: "OrderStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopOrders_OderStatusId",
                table: "ShopOrders",
                newName: "IX_ShopOrders_OrderStatusId");

            migrationBuilder.RenameColumn(
                name: "OderStatusId",
                table: "OrderStatuses",
                newName: "OrderStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_OrderStatuses_OrderStatusId",
                table: "ShopOrders",
                column: "OrderStatusId",
                principalTable: "OrderStatuses",
                principalColumn: "OrderStatusId",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_OrderStatuses_OrderStatusId",
                table: "ShopOrders");

            migrationBuilder.RenameColumn(
                name: "OrderStatusId",
                table: "ShopOrders",
                newName: "OderStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopOrders_OrderStatusId",
                table: "ShopOrders",
                newName: "IX_ShopOrders_OderStatusId");

            migrationBuilder.RenameColumn(
                name: "OrderStatusId",
                table: "OrderStatuses",
                newName: "OderStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_OrderStatuses_OderStatusId",
                table: "ShopOrders",
                column: "OderStatusId",
                principalTable: "OrderStatuses",
                principalColumn: "OderStatusId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
