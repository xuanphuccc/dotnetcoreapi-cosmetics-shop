using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class ProductOptionTypeDTO
	{
		public int OptionTypeId { get; set; }

		[Required]
		[StringLength(50)]
		public string Name { get; set; } = string.Empty;

		[Required]
		public List<ProductOptionDTO>? Options { get; set; }

	}

	public class ProductOptionDTO
	{
		public int ProductOptionId { get; set; }

		[StringLength(50)]
		[Required]
		public string Name { get; set; } = string.Empty;

		[StringLength(50)]
		[Required]
		public string Value { get; set; } = string.Empty;

		public int? OptionTypeId { get; set; }
	}
}
