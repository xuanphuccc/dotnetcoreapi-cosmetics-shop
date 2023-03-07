using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class CategoryDTO
	{
		[Required]
		[StringLength(100)]
		public string? Name { get; set; }

		public int? PromotionId { get; set; }
	}
}
