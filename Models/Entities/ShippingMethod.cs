using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class ShippingMethod
    {
        [Key]
        public int ShippingMethodId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal? Price { get; set; }

        public List<ShopOrder>? ShopOrders { get; set; }
    }
}
