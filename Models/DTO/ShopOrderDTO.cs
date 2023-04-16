using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class ShopOrderDTO
	{
		public int OrderId { get; set; }

		public DateTime OrderDate { get; set; }

		public decimal OrderTotal { get; set; }

        public decimal? ShippingCost { get; set; }

        public decimal? DiscountMoney { get; set; }

		public string? UserId { get; set; }
		public AppUserDTO? User { get; set; }

		public int? PaymentMethodId { get; set; }

		[Required]
		public int? AddressId { get; set; }

		public AddressDTO? Address { get; set; }

		public int? ShippingMethodId { get; set; }

		public int? OrderStatusId { get; set; }

		public List<OrderItemDTO>? Items { get; set; }
	}

	public class OrderItemDTO
	{
		public int OrderItemId { get; set; }

		[Required]
		public int Qty { get; set; }

		public decimal Price { get; set; }

        public int? DiscountRate { get; set; }

        public int? OrderId { get; set; }

		[Required]
		public int ProductItemId { get; set; }

		public ProductDTO? Product { get; set; }
	}
}
