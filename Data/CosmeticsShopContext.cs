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

            modelBuilder.Entity<Category>(entity =>
            {
                // Xóa Promotion -> SetNull Category
                entity.HasOne(c => c.Promotion)
                      .WithMany(p => p.Categories)
                      .OnDelete(DeleteBehavior.SetNull);
                // nếu muốn xóa thì dùng cascade
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                // trong 1 category không thể có 2 sản phẩm giống nhau
                entity.HasIndex(pc => new { pc.CategoryId, pc.ProductId })
                      .IsUnique();

                // Xóa Category -> Xóa ProductCategory
                entity.HasOne(pc => pc.Category)
                      .WithMany(c => c.ProductCategories)
                      .OnDelete(DeleteBehavior.Cascade);

                // Xóa Product -> Xóa ProductCategory
                entity.HasOne(pc => pc.Product)
                      .WithMany(p => p.ProductCategories)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Product>(entity =>
            {
                // Xóa Provider -> SetNull Product
                entity.HasOne(c => c.Provider)
                      .WithMany(p => p.Products)
                      .OnDelete(DeleteBehavior.SetNull);

            });



            modelBuilder.Entity<ProductItem>(entity =>
            {
                // Xóa Product -> xóa ProductItem
                entity.HasOne(pi => pi.Product)
                      .WithMany(p => p.ProductItems)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductConfiguration>(entity =>
            {
                // 1 ProductItem chỉ có 1 loại option
                // VD: 1 loại áo chỉ có 1 loại size M
                entity.HasIndex(pg => new { pg.ProductItemId, pg.ProductOptionId })
                      .IsUnique();

                // Xóa ProductItem -> xóa ProductConfiguration
                entity.HasOne(pg => pg.ProductItem)
                      .WithMany(pi => pi.ProductConfigurations)
                      .OnDelete(DeleteBehavior.Cascade);

                // Xóa ProductOption -> xóa ProductConfiguration
                entity.HasOne(pg => pg.ProductOption)
                      .WithMany(po => po.ProductConfigurations)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductOption>(entity =>
            {
                // Xóa ProductOptionType -> xóa ProductOption
                entity.HasOne(po => po.ProductOptionType)
                      .WithMany(pt => pt.ProductOptions)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                // Trong 1 giỏ hàng chỉ có 1 loại sản phẩm
                // VD: Chỉ có 1 áo size M với số lượng là 2
                // không phải 2 áo size M với số lượng là 1
                entity.HasIndex(sc => new { sc.CartId, sc.ProductItemId })
                      .IsUnique();

                // Xóa ProductItem -> Xóa ShoppingCartItem
                entity.HasOne(ci => ci.ProductItem)
                      .WithMany(pi => pi.ShoppingCartItems)
                      .OnDelete(DeleteBehavior.Cascade);

                // Xóa ShoppingCart -> Xóa ShoppingCartItem
                entity.HasOne(ci => ci.ShoppingCart)
                      .WithMany(pc => pc.Items)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                // Xóa User -> xóa ShoppingCart
                entity.HasOne(pc => pc.AppUser)
                      .WithMany(u => u.ShoppingCarts)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                // Xóa User -> xóa PaymentMethod
                entity.HasOne(pm => pm.AppUser)
                      .WithMany(u => u.PaymentMethods)
                      .OnDelete(DeleteBehavior.Cascade);

                // Xóa PaymentType -> xóa PaymentMethod
                entity.HasOne(pm => pm.PaymentType)
                      .WithMany(pt => pt.PaymentMethods)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Address>(entity =>
            {
                // Xóa User -> xóa Address
                entity.HasOne(a => a.AppUser)
                      .WithMany(u => u.Address)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                // 1 User và 1 Product chỉ xuất hiện 1 lần
                entity.HasIndex(w => new { w.UserId, w.ProductId })
                      .IsUnique();

                // Xóa User -> xóa Wishlist
                entity.HasOne(w => w.AppUser)
                      .WithMany(u => u.Wishlists)
                      .OnDelete(DeleteBehavior.Cascade);

                // Xóa Product -> xóa Wishlist
                entity.HasOne(w => w.Product)
                      .WithMany(p => p.Wishlists)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserReview>(entity =>
            {
                // Một sản phẩm đã mua chỉ có thể có 1 đánh giá
                entity.HasIndex(ur => new { ur.UserId, ur.OrderItemId })
                      .IsUnique();

                // Xóa User -> xóa UserReview
                entity.HasOne(ur => ur.AppUser)
                      .WithMany(u => u.UserReviews)
                      .OnDelete(DeleteBehavior.Cascade);

                // Xóa OrderItem -> xóa UserReview
                entity.HasOne(ur => ur.OrderItem)
                      .WithMany(ot => ot.UserReviews)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                // một giỏ hàng thì một loại sản phẩm
                // chỉ xuất hiện một lần
                entity.HasIndex(oi => new { oi.OrderId, oi.ProductItemId })
                      .IsUnique();

                // Xóa ProductItem -> xóa OrderItem
                entity.HasOne(oi => oi.ProductItem)
                      .WithMany(pi => pi.OrderItems)
                      .OnDelete(DeleteBehavior.Cascade);

                // Xóa ShopOrder -> xóa OrderItem
                entity.HasOne(oi => oi.ShopOrder)
                      .WithMany(so => so.Items)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ShopOrder>(entity =>
            {
                // Xóa User -> NoAction
                entity.HasOne(so => so.AppUser)
                      .WithMany(u => u.ShopOrders)
                      .OnDelete(DeleteBehavior.NoAction);

                // Xóa Address -> SetNull ShopOrder
                entity.HasOne(so => so.Address)
                      .WithMany(a => a.ShopOrders)
                      .OnDelete(DeleteBehavior.SetNull);

                // Xóa ShippingMethod -> SetNull ShopOrder
                entity.HasOne(so => so.ShippingMethod)
                      .WithMany(sm => sm.ShopOrders)
                      .OnDelete(DeleteBehavior.SetNull);

                // Xóa OrderStatus -> SetNull ShopOrder
                entity.HasOne(so => so.OrderStatus)
                      .WithMany(os => os.ShopOrders)
                      .OnDelete(DeleteBehavior.SetNull);

                // Xóa PaymentMethod -> NoAction do xung đột
                entity.HasOne(so => so.PaymentMethod)
                      .WithMany(pm => pm.ShopOrders)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AdminRole>(entity =>
            {
                entity.HasIndex(ar => new { ar.RoleId, ar.AdminUserId })
                      .IsUnique();

                entity.HasOne(ar => ar.Role)
                      .WithMany(r => r.AdminRoles)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ar => ar.AdminUser)
                      .WithMany(au => au.AdminRoles)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<ProductConfiguration> ProductConfigurations { get; set; }
        public DbSet<ProductOptionType> ProductOptionTypes { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<ShopOrder> ShopOrders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }

        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AdminRole> AdminRoles { get; set; }
    }
}
