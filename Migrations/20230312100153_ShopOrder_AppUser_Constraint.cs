using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_api_cosmetics_shop.Migrations
{
    public partial class ShopOrder_AppUser_Constraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ShopOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrders_UserId",
                table: "ShopOrders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_AppUsers_UserId",
                table: "ShopOrders",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_AppUsers_UserId",
                table: "ShopOrders");

            migrationBuilder.DropIndex(
                name: "IX_ShopOrders_UserId",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShopOrders");
        }
    }
}
