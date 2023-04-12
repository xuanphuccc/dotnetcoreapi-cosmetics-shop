using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class ProductConfiguration
    {
        [Key]
        public int ConfigurationId { get; set; }

        public int? ProductItemId { get; set; }
        [ForeignKey("ProductItemId")]
        public ProductItem? ProductItem { get; set; }

        public int? ProductOptionId { get; set; }
        [ForeignKey("ProductOptionId")]
        public ProductOption? ProductOption { get; set; }
    }
}
