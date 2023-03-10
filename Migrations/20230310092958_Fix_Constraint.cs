using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_api_cosmetics_shop.Migrations
{
    public partial class Fix_Constraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AppUsers_UserId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductItems_ProductItemId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ShopOrders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_AppUsers_UserId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_PaymentTypes_PaymentTypeId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Categories_CategoryId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Products_ProductId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductConfigurations_ProductItems_ProductItemId",
                table: "ProductConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductConfigurations_ProductOptions_ProductOptionId",
                table: "ProductConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductItems_Products_ProductId",
                table: "ProductItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOptions_ProductOptionTypes_OptionTypeId",
                table: "ProductOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_Addresses_AddressId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_OrderStatuses_OderStatusId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_PaymentMethods_PaymentMethodId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_ShippingMethods_ShippingMethodId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_ProductItems_ProductItemId",
                table: "ShoppingCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_ShoppingCarts_CartId",
                table: "ShoppingCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_AppUsers_UserId",
                table: "ShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReviews_AppUsers_UserId",
                table: "UserReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReviews_OrderItems_OrderItemId",
                table: "UserReviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AppUsers_UserId",
                table: "Addresses",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductItems_ProductItemId",
                table: "OrderItems",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "ProductItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ShopOrders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "ShopOrders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_AppUsers_UserId",
                table: "PaymentMethods",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_PaymentTypes_PaymentTypeId",
                table: "PaymentMethods",
                column: "PaymentTypeId",
                principalTable: "PaymentTypes",
                principalColumn: "PaymentTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Categories_CategoryId",
                table: "ProductCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Products_ProductId",
                table: "ProductCategories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductConfigurations_ProductItems_ProductItemId",
                table: "ProductConfigurations",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "ProductItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductConfigurations_ProductOptions_ProductOptionId",
                table: "ProductConfigurations",
                column: "ProductOptionId",
                principalTable: "ProductOptions",
                principalColumn: "ProductOptionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItems_Products_ProductId",
                table: "ProductItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOptions_ProductOptionTypes_OptionTypeId",
                table: "ProductOptions",
                column: "OptionTypeId",
                principalTable: "ProductOptionTypes",
                principalColumn: "OptionTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_Addresses_AddressId",
                table: "ShopOrders",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_OrderStatuses_OderStatusId",
                table: "ShopOrders",
                column: "OderStatusId",
                principalTable: "OrderStatuses",
                principalColumn: "OderStatusId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_PaymentMethods_PaymentMethodId",
                table: "ShopOrders",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_ShippingMethods_ShippingMethodId",
                table: "ShopOrders",
                column: "ShippingMethodId",
                principalTable: "ShippingMethods",
                principalColumn: "ShippingMethodId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_ProductItems_ProductItemId",
                table: "ShoppingCartItems",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "ProductItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_ShoppingCarts_CartId",
                table: "ShoppingCartItems",
                column: "CartId",
                principalTable: "ShoppingCarts",
                principalColumn: "CartId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_AppUsers_UserId",
                table: "ShoppingCarts",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviews_AppUsers_UserId",
                table: "UserReviews",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviews_OrderItems_OrderItemId",
                table: "UserReviews",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "OrderItemId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AppUsers_UserId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductItems_ProductItemId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ShopOrders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_AppUsers_UserId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_PaymentTypes_PaymentTypeId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Categories_CategoryId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_Products_ProductId",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductConfigurations_ProductItems_ProductItemId",
                table: "ProductConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductConfigurations_ProductOptions_ProductOptionId",
                table: "ProductConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductItems_Products_ProductId",
                table: "ProductItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOptions_ProductOptionTypes_OptionTypeId",
                table: "ProductOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_Addresses_AddressId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_OrderStatuses_OderStatusId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_PaymentMethods_PaymentMethodId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_ShippingMethods_ShippingMethodId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_ProductItems_ProductItemId",
                table: "ShoppingCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_ShoppingCarts_CartId",
                table: "ShoppingCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_AppUsers_UserId",
                table: "ShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReviews_AppUsers_UserId",
                table: "UserReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReviews_OrderItems_OrderItemId",
                table: "UserReviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AppUsers_UserId",
                table: "Addresses",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductItems_ProductItemId",
                table: "OrderItems",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "ProductItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ShopOrders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "ShopOrders",
                principalColumn: "OrderId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Categories_CategoryId",
                table: "ProductCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_Products_ProductId",
                table: "ProductCategories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductConfigurations_ProductItems_ProductItemId",
                table: "ProductConfigurations",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "ProductItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductConfigurations_ProductOptions_ProductOptionId",
                table: "ProductConfigurations",
                column: "ProductOptionId",
                principalTable: "ProductOptions",
                principalColumn: "ProductOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItems_Products_ProductId",
                table: "ProductItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOptions_ProductOptionTypes_OptionTypeId",
                table: "ProductOptions",
                column: "OptionTypeId",
                principalTable: "ProductOptionTypes",
                principalColumn: "OptionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_Addresses_AddressId",
                table: "ShopOrders",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_OrderStatuses_OderStatusId",
                table: "ShopOrders",
                column: "OderStatusId",
                principalTable: "OrderStatuses",
                principalColumn: "OderStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_PaymentMethods_PaymentMethodId",
                table: "ShopOrders",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_ShippingMethods_ShippingMethodId",
                table: "ShopOrders",
                column: "ShippingMethodId",
                principalTable: "ShippingMethods",
                principalColumn: "ShippingMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_ProductItems_ProductItemId",
                table: "ShoppingCartItems",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "ProductItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_ShoppingCarts_CartId",
                table: "ShoppingCartItems",
                column: "CartId",
                principalTable: "ShoppingCarts",
                principalColumn: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_AppUsers_UserId",
                table: "ShoppingCarts",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviews_AppUsers_UserId",
                table: "UserReviews",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviews_OrderItems_OrderItemId",
                table: "UserReviews",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "OrderItemId");
        }
    }
}
