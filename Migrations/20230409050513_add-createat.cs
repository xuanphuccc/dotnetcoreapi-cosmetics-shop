using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_api_cosmetics_shop.Migrations
{
    public partial class addcreateat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Wishlists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "ShoppingCartItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "ShippingMethods",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Providers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Promotions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "PaymentMethods",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Addresses",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "ShippingMethods");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Addresses");
        }
    }
}
