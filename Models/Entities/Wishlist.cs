using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
	public class Wishlist
	{
		[Key]
		public int WishlistId { get; set; }

		public string? UserId { get; set; }
		[ForeignKey("UserId")]
		public AppUser? AppUser { get; set; }

		public int? ProductId { get; set; }
		[ForeignKey("ProductId")]
		public Product? Product { get; set; }

        DateTime? CreateAt { get; set; }
    }
}
