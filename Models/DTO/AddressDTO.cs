using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class AddressDTO
	{
		public int AddressId { get; set; }

		[Required]
		[StringLength(50)]
		public string FullName { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string City { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string District { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string Ward { get; set; } = string.Empty;

		[Required]
		[StringLength(256)]
		public string AddressLine { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required]
		public bool IsDefault { get; set; }

        DateTime? CreateAt { get; set; }

        public string? UserId { get; set; }
	}
}
