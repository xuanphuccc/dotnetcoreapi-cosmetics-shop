using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class ShopOrder
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal OrderTotal { get; set; }

		public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }

		public int? PaymentMethodId { get; set; }
        [ForeignKey("PaymentMethodId")]
        public PaymentMethod? PaymentMethod { get; set; }

        public int? AddressId { get; set; }
        [ForeignKey("AddressId")]
        public Address? Address { get; set; }

        public int? ShippingMethodId { get; set; }
        [ForeignKey("ShippingMethodId")]
        public ShippingMethod? ShippingMethod { get; set; }

        public int? OrderStatusId { get; set; }
        [ForeignKey("OrderStatusId")]
        public OrderStatus? OrderStatus { get; set; }

        public List<OrderItem>? Items { get; set; }
    }
}
