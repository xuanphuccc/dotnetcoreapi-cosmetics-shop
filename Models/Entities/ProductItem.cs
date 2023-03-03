using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class ProductItem
    {
        [Key]
        public int ProductItemId { get; set; }

        [StringLength(50)]
        public string? SKU { get; set; }
        public int QtyInStock { get; set; }
        public string? Image { get; set; }
        public decimal? Price { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public List<ProductConfiguration>? ProductConfigurations { get; set; }
    }
}
