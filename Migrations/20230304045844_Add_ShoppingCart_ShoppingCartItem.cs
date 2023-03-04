using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asp_dotnet_core_web_api_cosmetics_shop.Migrations
{
    public partial class Add_ShoppingCart_ShoppingCartItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethod_AppUsers_UserId",
                table: "PaymentMethod");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethod_PaymentTypes_PaymentTypeId",
                table: "PaymentMethod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentMethod",
                table: "PaymentMethod");

            migrationBuilder.RenameTable(
                name: "PaymentMethod",
                newName: "PaymentMethods");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentMethod_UserId",
                table: "PaymentMethods",
                newName: "IX_PaymentMethods_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentMethod_PaymentTypeId",
                table: "PaymentMethods",
                newName: "IX_PaymentMethods_PaymentTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentMethods",
                table: "PaymentMethods",
                column: "PaymentMethodId");

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCartItems",
                columns: table => new
                {
                    CartItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    CartId = table.Column<int>(type: "int", nullable: true),
                    ProductItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartItems", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_ShoppingCartItems_ProductItems_ProductItemId",
                        column: x => x.ProductItemId,
                        principalTable: "ProductItems",
                        principalColumn: "ProductItemId");
                    table.ForeignKey(
                        name: "FK_ShoppingCartItems_ShoppingCarts_CartId",
                        column: x => x.CartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "CartId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_CartId_ProductItemId",
                table: "ShoppingCartItems",
                columns: new[] { "CartId", "ProductItemId" },
                unique: true,
                filter: "[CartId] IS NOT NULL AND [ProductItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_ProductItemId",
                table: "ShoppingCartItems",
                column: "ProductItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_UserId",
                table: "ShoppingCarts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_AppUsers_UserId",
                table: "PaymentMethods",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_PaymentTypes_PaymentTypeId",
                table: "PaymentMethods",
                column: "PaymentTypeId",
                principalTable: "PaymentTypes",
                principalColumn: "PaymentTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_AppUsers_UserId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_PaymentTypes_PaymentTypeId",
                table: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "ShoppingCartItems");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentMethods",
                table: "PaymentMethods");

            migrationBuilder.RenameTable(
                name: "PaymentMethods",
                newName: "PaymentMethod");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentMethods_UserId",
                table: "PaymentMethod",
                newName: "IX_PaymentMethod_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentMethods_PaymentTypeId",
                table: "PaymentMethod",
                newName: "IX_PaymentMethod_PaymentTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentMethod",
                table: "PaymentMethod",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethod_AppUsers_UserId",
                table: "PaymentMethod",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethod_PaymentTypes_PaymentTypeId",
                table: "PaymentMethod",
                column: "PaymentTypeId",
                principalTable: "PaymentTypes",
                principalColumn: "PaymentTypeId");
        }
    }
}
