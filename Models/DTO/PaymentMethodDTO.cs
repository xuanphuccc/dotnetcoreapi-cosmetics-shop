using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class PaymentMethodDTO
	{
		public int PaymentMethodId { get; set; }

		[Required]
		[StringLength(100)]
		public string Provider { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string AccountNumber { get; set; } = string.Empty;

		[Required]
		public DateTime ExpiryDate { get; set; }

		[Required]
		public bool IsDefault { get; set; }

		public int? PaymentTypeId { get; set; }

		public string? UserId { get; set; }
	}
}
