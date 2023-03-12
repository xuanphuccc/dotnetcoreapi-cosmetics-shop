using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class ShoppingCartDTO
	{
		public int CartId { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;

		[Required]
		public List<ShoppingCartItemDTO>? Items { get; set; }
	}

	public class ShoppingCartItemDTO
	{
		public int CartItemId { get; set; }

		[Required]
		public int Qty { get; set; }

		[Required]
		public int CartId { get; set; }

		[Required]
		public int ProductItemId { get; set; }

		public ProductDTO? Product { get; set; }
	}
}
