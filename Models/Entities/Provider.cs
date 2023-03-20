using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class Provider
    {
        [Key]
        public int ProviderId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public List<Product>? Products { get; set; }
    }
}
