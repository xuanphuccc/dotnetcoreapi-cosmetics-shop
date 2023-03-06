using web_api_cosmetics_shop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace web_api_cosmetics_shop.Data
{
    public class CosmeticsShopContext : DbContext
    {
        public CosmeticsShopContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductCategory>(entity => {
                // trong 1 category không thể có 2 sản phẩm giống nhau
                entity.HasIndex(productCategory => new { productCategory.CategoryId, productCategory.ProductId })
                      .IsUnique();
            });

            modelBuilder.Entity<ProductConfiguration>(entity =>
            {
                // 1 ProductItem chỉ có 1 loại option
                // VD: 1 loại áo chỉ có 1 loại size M
                entity.HasIndex(productConfiguration => new { productConfiguration.ProductItemId, productConfiguration.ProductOptionId })
                      .IsUnique();
            });

            modelBuilder.Entity<ShoppingCartItem>(entity => {
                // Trong 1 giỏ hàng chỉ có 1 loại sản phẩm
                // VD: Chỉ có 1 áo size M với số lượng là 2
                // không phải 2 áo size M với số lượng là 1
                entity.HasIndex(shoppingCartItem => new { shoppingCartItem.CartId, shoppingCartItem.ProductItemId })
                      .IsUnique();
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasIndex(orderItem => new {orderItem.OrderId, orderItem.ProductItemId})
                      .IsUnique();
            });

            modelBuilder.Entity<UserReview>(entity =>
            {
                // Một sản phẩm đã mua chỉ có thể có 1 đánh giá
                entity.HasIndex(orderItem => new { orderItem.UserId, orderItem.OrderItemId })
                      .IsUnique();
            });
        }

        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<ProductConfiguration> ProductConfigurations { get; set; }
        public DbSet<ProductOptionType> ProductOptionTypes { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set;}
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set;}

        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<ShopOrder> ShopOrders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }
    }
}
