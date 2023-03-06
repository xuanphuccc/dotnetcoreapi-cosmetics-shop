using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_api_cosmetics_shop.Migrations
{
    public partial class Add_Index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserReviews_UserId",
                table: "UserReviews");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_UserId_OrderItemId",
                table: "UserReviews",
                columns: new[] { "UserId", "OrderItemId" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [OrderItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_ProductItemId",
                table: "OrderItems",
                columns: new[] { "OrderId", "ProductItemId" },
                unique: true,
                filter: "[OrderId] IS NOT NULL AND [ProductItemId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserReviews_UserId_OrderItemId",
                table: "UserReviews");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId_ProductItemId",
                table: "OrderItems");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_UserId",
                table: "UserReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }
    }
}
