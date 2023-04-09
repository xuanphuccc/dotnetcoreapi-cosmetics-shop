using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
    public class ProviderDTO
    {
        public int ProviderId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime? CreateAt { get; set; }
    }
}
