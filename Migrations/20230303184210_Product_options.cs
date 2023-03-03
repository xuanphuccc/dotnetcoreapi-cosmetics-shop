using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asp_dotnet_core_web_api_cosmetics_shop.Migrations
{
    public partial class Product_options : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "ProductItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProductOptions",
                columns: table => new
                {
                    ProductOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOptions", x => x.ProductOptionId);
                });

            migrationBuilder.CreateTable(
                name: "ProductConfigurations",
                columns: table => new
                {
                    ConfigurationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductItemId = table.Column<int>(type: "int", nullable: true),
                    ProductOptionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductConfigurations", x => x.ConfigurationId);
                    table.ForeignKey(
                        name: "FK_ProductConfigurations_ProductItems_ProductItemId",
                        column: x => x.ProductItemId,
                        principalTable: "ProductItems",
                        principalColumn: "ProductItemId");
                    table.ForeignKey(
                        name: "FK_ProductConfigurations_ProductOptions_ProductOptionId",
                        column: x => x.ProductOptionId,
                        principalTable: "ProductOptions",
                        principalColumn: "ProductOptionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductConfigurations_ProductItemId_ProductOptionId",
                table: "ProductConfigurations",
                columns: new[] { "ProductItemId", "ProductOptionId" },
                unique: true,
                filter: "[ProductItemId] IS NOT NULL AND [ProductOptionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConfigurations_ProductOptionId",
                table: "ProductConfigurations",
                column: "ProductOptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductConfigurations");

            migrationBuilder.DropTable(
                name: "ProductOptions");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "ProductItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
