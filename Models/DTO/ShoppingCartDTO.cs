using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class ShoppingCartDTO
	{
		public int CartId { get; set; }

		public string? UserId { get; set; }

		[Required]
		public List<ShoppingCartItemDTO>? Items { get; set; }
	}

	public class ShoppingCartItemDTO
	{
		public int CartItemId { get; set; }

		[Required]
		public int Qty { get; set; }

		public int? CartId { get; set; }

		public int? ProductItemId { get; set; }
	}
}
