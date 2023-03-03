using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class ProductConfiguration
    {
        [Key]
        public int ConfigurationId { get; set; }

        public int? ProductItemId { get; set; }
        public ProductItem? ProductItem { get; set; }

        public int? ProductOptionId { get; set; }
        public ProductOption? ProductOption { get; set; }
    }
}
