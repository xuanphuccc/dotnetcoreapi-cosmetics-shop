using System.ComponentModel.DataAnnotations;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class ProductOption
    {
        [Key]
        public int ProductOptionId { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? Value { get; set; }

        public List<ProductConfiguration>? ProductConfigurations { get; set; }
    }
}
