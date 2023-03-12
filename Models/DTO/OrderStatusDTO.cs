using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class OrderStatusDTO
	{
		public int OrderStatusId { get; set; }

		[Required]
		[StringLength(100)]
		public string Status { get; set; } = string.Empty;
	}
}
