using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class ProductItem
    {
        [Key]
        public int ProductItemId { get; set; }

        [StringLength(50)]
        [Required]
        public string? SKU { get; set; }

        [Required]
        public int QtyInStock { get; set; }

        public string? Image { get; set; }

        [Required]
        public decimal? Price { get; set; }

        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public List<ProductConfiguration>? ProductConfigurations { get; set; }
        public List<ShoppingCartItem>? ShoppingCartItems { get; set;}
        public List<OrderItem>? OrderItems { get; set; }
    }
}
