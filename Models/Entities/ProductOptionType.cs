using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class ProductOptionType
    {
        [Key]
        public int OptionTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        
        public List<ProductOption>? ProductOptions { get; set; }
    }
}
