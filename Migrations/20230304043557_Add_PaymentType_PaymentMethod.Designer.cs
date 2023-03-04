﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using asp_dotnet_core_web_api_cosmetics_shop.Data;

#nullable disable

namespace asp_dotnet_core_web_api_cosmetics_shop.Migrations
{
    [DbContext(typeof(CosmeticsShopContext))]
    [Migration("20230304043557_Add_PaymentType_PaymentMethod")]
    partial class Add_PaymentType_PaymentMethod
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Address", b =>
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

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.AppUser", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId");

                    b.ToTable("AppUsers");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

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

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.PaymentMethod", b =>
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

                    b.ToTable("PaymentMethod");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.PaymentType", b =>
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

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("ntext");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductCategory", b =>
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

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductConfiguration", b =>
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

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductItem", b =>
                {
                    b.Property<int>("ProductItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductItemId"), 1L, 1);

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

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductOption", b =>
                {
                    b.Property<int>("ProductOptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductOptionId"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ProductOptionId");

                    b.ToTable("ProductOptions");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Promotion", b =>
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

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Address", b =>
                {
                    b.HasOne("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.AppUser", "AppUser")
                        .WithMany("Address")
                        .HasForeignKey("UserId");

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Category", b =>
                {
                    b.HasOne("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Promotion", "Promotion")
                        .WithMany("Categories")
                        .HasForeignKey("PromotionId");

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.PaymentMethod", b =>
                {
                    b.HasOne("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.PaymentType", "PaymentType")
                        .WithMany("PaymentMethods")
                        .HasForeignKey("PaymentTypeId");

                    b.HasOne("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.AppUser", "AppUser")
                        .WithMany("PaymentMethods")
                        .HasForeignKey("UserId");

                    b.Navigation("AppUser");

                    b.Navigation("PaymentType");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductCategory", b =>
                {
                    b.HasOne("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Category", "Category")
                        .WithMany("ProductCategories")
                        .HasForeignKey("CategoryId");

                    b.HasOne("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Product", "Product")
                        .WithMany("ProductCategories")
                        .HasForeignKey("ProductId");

                    b.Navigation("Category");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductConfiguration", b =>
                {
                    b.HasOne("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductItem", "ProductItem")
                        .WithMany("ProductConfigurations")
                        .HasForeignKey("ProductItemId");

                    b.HasOne("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductOption", "ProductOption")
                        .WithMany("ProductConfigurations")
                        .HasForeignKey("ProductOptionId");

                    b.Navigation("ProductItem");

                    b.Navigation("ProductOption");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductItem", b =>
                {
                    b.HasOne("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Product", "Product")
                        .WithMany("ProductItems")
                        .HasForeignKey("ProductId");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.AppUser", b =>
                {
                    b.Navigation("Address");

                    b.Navigation("PaymentMethods");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Category", b =>
                {
                    b.Navigation("ProductCategories");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.PaymentType", b =>
                {
                    b.Navigation("PaymentMethods");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Product", b =>
                {
                    b.Navigation("ProductCategories");

                    b.Navigation("ProductItems");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductItem", b =>
                {
                    b.Navigation("ProductConfigurations");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.ProductOption", b =>
                {
                    b.Navigation("ProductConfigurations");
                });

            modelBuilder.Entity("asp_dotnet_core_web_api_cosmetics_shop.Models.Entities.Promotion", b =>
                {
                    b.Navigation("Categories");
                });
#pragma warning restore 612, 618
        }
    }
}
