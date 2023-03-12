﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class ShopOrderDTO
	{
		public int OrderId { get; set; }

		public DateTime OrderDate { get; set; }

		public decimal OrderTotal { get; set; }

		[Required]
		public string? UserId { get; set; }

		[Required]
		public int? PaymentMethodId { get; set; }

		[Required]
		public int? AddressId { get; set; }

		[Required]
		public int? ShippingMethodId { get; set; }

		[Required]
		public int? OrderStatusId { get; set; }

		public List<OrderItemDTO>? Items { get; set; }
	}

	public class OrderItemDTO
	{
		public int OrderItemId { get; set; }

		[Required]
		public int Qty { get; set; }

		public decimal Price { get; set; }

		public int? OrderId { get; set; }

		[Required]
		public int? ProductItemId { get; set; }

		public ProductDTO? Product { get; set; }
	}
}
