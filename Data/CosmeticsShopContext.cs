using asp_dotnet_core_web_api_cosmetics_shop.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace asp_dotnet_core_web_api_cosmetics_shop.Data
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
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
    }
}
