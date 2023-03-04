using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int Qty { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public ShopOrder? ShopOrder { get; set; }

        public int? ProductItemId { get; set; }
        [ForeignKey("ProductItemId")]
        public ProductItem? ProductItem { get; set; }

        public List<UserReview>? UserReviews { get; set; }
    }
}
