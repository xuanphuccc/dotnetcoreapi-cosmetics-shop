using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class ProductOption
    {
        [Key]
        public int ProductOptionId { get; set; }

        [StringLength(50)]
        [Required]
        public string? Name { get; set; }

        [StringLength(50)]
        [Required]
        public string? Value { get; set; }


        public int? OptionTypeId { get; set; }
        [ForeignKey("OptionTypeId")]
        public ProductOptionType? ProductOptionType { get; set; }

        public List<ProductConfiguration>? ProductConfigurations { get; set; }
    }
}
