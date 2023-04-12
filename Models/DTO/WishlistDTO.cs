using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class WishlistDTO
	{
		public int WishlistId { get; set; }

		public string? UserId { get; set; }

		[Required]
		public int? ProductId { get; set; }

		public ProductDTO? Product { get; set; }

		public DateTime? CreateAt { get; set; }
	}
}
