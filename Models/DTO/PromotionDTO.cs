using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class PromotionDTO
	{
		[Required]
		[StringLength(256)]
		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }

		[Required]
		[Range(1, 100)]
		public int DiscountRate { get; set; }

		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		public DateTime EndDate { get; set; }
	}
}
