using System.ComponentModel.DataAnnotations;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class OrderStatus
    {
        [Key]
        public int OderStatusId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Status { get; set; }

        public List<ShopOrder> ShopOrders { get; set; }
    }
}
