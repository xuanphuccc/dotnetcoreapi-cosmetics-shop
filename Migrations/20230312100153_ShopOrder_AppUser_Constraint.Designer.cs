﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using web_api_cosmetics_shop.Data;

#nullable disable

namespace web_api_cosmetics_shop.Migrations
{
    [DbContext(typeof(CosmeticsShopContext))]
    [Migration("20230312100153_ShopOrder_AppUser_Constraint")]
    partial class ShopOrder_AppUser_Constraint
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AddressId"), 1L, 1);

                    b.Property<string>("AddressLine")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("District")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Ward")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("AddressId");

                    b.HasIndex("UserId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.AppUser", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId");

                    b.ToTable("AppUsers");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("PromotionId")
                        .HasColumnType("int");

                    b.HasKey("CategoryId");

                    b.HasIndex("PromotionId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.OrderItem", b =>
                {
                    b.Property<int>("OrderItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderItemId"), 1L, 1);

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("ProductItemId")
                        .HasColumnType("int");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.HasKey("OrderItemId");

                    b.HasIndex("ProductItemId");

                    b.HasIndex("OrderId", "ProductItemId")
                        .IsUnique()
                        .HasFilter("[OrderId] IS NOT NULL AND [ProductItemId] IS NOT NULL");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.OrderStatus", b =>
                {
                    b.Property<int>("OderStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OderStatusId"), 1L, 1);

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("OderStatusId");

                    b.ToTable("OrderStatuses");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.PaymentMethod", b =>
                {
                    b.Property<int>("PaymentMethodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentMethodId"), 1L, 1);

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<int?>("PaymentTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PaymentMethodId");

                    b.HasIndex("PaymentTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("PaymentMethods");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.PaymentType", b =>
                {
                    b.Property<int>("PaymentTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentTypeId"), 1L, 1);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("PaymentTypeId");

                    b.ToTable("PaymentTypes");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("ntext");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDisplay")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductCategory", b =>
                {
                    b.Property<int>("ProductCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductCategoryId"), 1L, 1);

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("ProductCategoryId");

                    b.HasIndex("ProductId");

                    b.HasIndex("CategoryId", "ProductId")
                        .IsUnique()
                        .HasFilter("[CategoryId] IS NOT NULL AND [ProductId] IS NOT NULL");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductConfiguration", b =>
                {
                    b.Property<int>("ConfigurationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ConfigurationId"), 1L, 1);

                    b.Property<int?>("ProductItemId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductOptionId")
                        .HasColumnType("int");

                    b.HasKey("ConfigurationId");

                    b.HasIndex("ProductOptionId");

                    b.HasIndex("ProductItemId", "ProductOptionId")
                        .IsUnique()
                        .HasFilter("[ProductItemId] IS NOT NULL AND [ProductOptionId] IS NOT NULL");

                    b.ToTable("ProductConfigurations");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductItem", b =>
                {
                    b.Property<int>("ProductItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductItemId"), 1L, 1);

                    b.Property<decimal?>("CostPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Price")
                        .IsRequired()
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("QtyInStock")
                        .HasColumnType("int");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ProductItemId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductItems");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductOption", b =>
                {
                    b.Property<int>("ProductOptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductOptionId"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("OptionTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ProductOptionId");

                    b.HasIndex("OptionTypeId");

                    b.ToTable("ProductOptions");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductOptionType", b =>
                {
                    b.Property<int>("OptionTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OptionTypeId"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("OptionTypeId");

                    b.ToTable("ProductOptionTypes");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Promotion", b =>
                {
                    b.Property<int>("PromotionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PromotionId"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("ntext");

                    b.Property<int>("DiscountRate")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("PromotionId");

                    b.ToTable("Promotions");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ShippingMethod", b =>
                {
                    b.Property<int>("ShippingMethodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ShippingMethodId"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal?>("Price")
                        .IsRequired()
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ShippingMethodId");

                    b.ToTable("ShippingMethods");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ShopOrder", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"), 1L, 1);

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<int?>("OderStatusId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("OrderTotal")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("PaymentMethodId")
                        .HasColumnType("int");

                    b.Property<int?>("ShippingMethodId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("OrderId");

                    b.HasIndex("AddressId");

                    b.HasIndex("OderStatusId");

                    b.HasIndex("PaymentMethodId");

                    b.HasIndex("ShippingMethodId");

                    b.HasIndex("UserId");

                    b.ToTable("ShopOrders");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ShoppingCart", b =>
                {
                    b.Property<int>("CartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartId"), 1L, 1);

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CartId");

                    b.HasIndex("UserId");

                    b.ToTable("ShoppingCarts");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ShoppingCartItem", b =>
                {
                    b.Property<int>("CartItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartItemId"), 1L, 1);

                    b.Property<int?>("CartId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductItemId")
                        .HasColumnType("int");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.HasKey("CartItemId");

                    b.HasIndex("ProductItemId");

                    b.HasIndex("CartId", "ProductItemId")
                        .IsUnique()
                        .HasFilter("[CartId] IS NOT NULL AND [ProductItemId] IS NOT NULL");

                    b.ToTable("ShoppingCartItems");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.UserReview", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReviewId"), 1L, 1);

                    b.Property<string>("Comment")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CommentDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("OrderItemId")
                        .HasColumnType("int");

                    b.Property<double>("RatingValue")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ReviewId");

                    b.HasIndex("OrderItemId");

                    b.HasIndex("UserId", "OrderItemId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL AND [OrderItemId] IS NOT NULL");

                    b.ToTable("UserReviews");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Wishlist", b =>
                {
                    b.Property<int>("WishlistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WishlistId"), 1L, 1);

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("WishlistId");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId", "ProductId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL AND [ProductId] IS NOT NULL");

                    b.ToTable("Wishlists");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Address", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.AppUser", "AppUser")
                        .WithMany("Address")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Category", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.Promotion", "Promotion")
                        .WithMany("Categories")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.OrderItem", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.ShopOrder", "ShopOrder")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.ProductItem", "ProductItem")
                        .WithMany("OrderItems")
                        .HasForeignKey("ProductItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("ProductItem");

                    b.Navigation("ShopOrder");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.PaymentMethod", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.PaymentType", "PaymentType")
                        .WithMany("PaymentMethods")
                        .HasForeignKey("PaymentTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.AppUser", "AppUser")
                        .WithMany("PaymentMethods")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("AppUser");

                    b.Navigation("PaymentType");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductCategory", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.Category", "Category")
                        .WithMany("ProductCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.Product", "Product")
                        .WithMany("ProductCategories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Category");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductConfiguration", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.ProductItem", "ProductItem")
                        .WithMany("ProductConfigurations")
                        .HasForeignKey("ProductItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.ProductOption", "ProductOption")
                        .WithMany("ProductConfigurations")
                        .HasForeignKey("ProductOptionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("ProductItem");

                    b.Navigation("ProductOption");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductItem", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.Product", "Product")
                        .WithMany("ProductItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Product");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductOption", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.ProductOptionType", "ProductOptionType")
                        .WithMany("ProductOptions")
                        .HasForeignKey("OptionTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("ProductOptionType");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ShopOrder", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.Address", "Address")
                        .WithMany("ShopOrders")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.OrderStatus", "OrderStatus")
                        .WithMany("ShopOrders")
                        .HasForeignKey("OderStatusId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.PaymentMethod", "PaymentMethod")
                        .WithMany("ShopOrders")
                        .HasForeignKey("PaymentMethodId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.ShippingMethod", "ShippingMethod")
                        .WithMany("ShopOrders")
                        .HasForeignKey("ShippingMethodId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.AppUser", "AppUser")
                        .WithMany("ShopOrders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Address");

                    b.Navigation("AppUser");

                    b.Navigation("OrderStatus");

                    b.Navigation("PaymentMethod");

                    b.Navigation("ShippingMethod");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ShoppingCart", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.AppUser", "AppUser")
                        .WithMany("ShoppingCarts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ShoppingCartItem", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.ShoppingCart", "ShoppingCart")
                        .WithMany("Items")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.ProductItem", "ProductItem")
                        .WithMany("ShoppingCartItems")
                        .HasForeignKey("ProductItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("ProductItem");

                    b.Navigation("ShoppingCart");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.UserReview", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.OrderItem", "OrderItem")
                        .WithMany("UserReviews")
                        .HasForeignKey("OrderItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.AppUser", "AppUser")
                        .WithMany("UserReviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("AppUser");

                    b.Navigation("OrderItem");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Wishlist", b =>
                {
                    b.HasOne("web_api_cosmetics_shop.Models.Entities.Product", "Product")
                        .WithMany("Wishlists")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("web_api_cosmetics_shop.Models.Entities.AppUser", "AppUser")
                        .WithMany("Wishlists")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("AppUser");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Address", b =>
                {
                    b.Navigation("ShopOrders");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.AppUser", b =>
                {
                    b.Navigation("Address");

                    b.Navigation("PaymentMethods");

                    b.Navigation("ShopOrders");

                    b.Navigation("ShoppingCarts");

                    b.Navigation("UserReviews");

                    b.Navigation("Wishlists");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Category", b =>
                {
                    b.Navigation("ProductCategories");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.OrderItem", b =>
                {
                    b.Navigation("UserReviews");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.OrderStatus", b =>
                {
                    b.Navigation("ShopOrders");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.PaymentMethod", b =>
                {
                    b.Navigation("ShopOrders");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.PaymentType", b =>
                {
                    b.Navigation("PaymentMethods");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Product", b =>
                {
                    b.Navigation("ProductCategories");

                    b.Navigation("ProductItems");

                    b.Navigation("Wishlists");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductItem", b =>
                {
                    b.Navigation("OrderItems");

                    b.Navigation("ProductConfigurations");

                    b.Navigation("ShoppingCartItems");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductOption", b =>
                {
                    b.Navigation("ProductConfigurations");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ProductOptionType", b =>
                {
                    b.Navigation("ProductOptions");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.Promotion", b =>
                {
                    b.Navigation("Categories");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ShippingMethod", b =>
                {
                    b.Navigation("ShopOrders");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ShopOrder", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("web_api_cosmetics_shop.Models.Entities.ShoppingCart", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
