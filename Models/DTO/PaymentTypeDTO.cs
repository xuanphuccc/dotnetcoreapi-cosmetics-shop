using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class PaymentTypeDTO
	{
		public int PaymentTypeId { get; set; }

		[Required]
		[StringLength(50)]
		public string Value { get; set; } = string.Empty;
	}
}
