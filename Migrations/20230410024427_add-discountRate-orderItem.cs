using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_api_cosmetics_shop.Migrations
{
    public partial class adddiscountRateorderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountRate",
                table: "OrderItems",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountRate",
                table: "OrderItems");
        }
    }
}
