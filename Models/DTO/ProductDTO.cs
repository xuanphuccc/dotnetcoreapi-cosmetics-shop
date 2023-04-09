using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
	public class ProductDTO
	{
		public int ProductId { get; set; }

		[StringLength(450)]
		[Required]
		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }
		public string? Image { get; set; }

		[Required]
		public bool IsDisplay { get; set; }

        public DateTime? CreateAt { get; set; }

        public int? ProviderId { get; set; }

        [Required]
		public List<int>? CategoriesId { get; set; }

		[Required]
		public List<ProductItemDTO>? Items  { get; set; }
	}

	public class ProductItemDTO
	{
		public int ProductItemId { get; set; }

		[StringLength(50)]
		[Required]
		public string SKU { get; set; } = string.Empty;

		[Required]
		public int QtyInStock { get; set; }

		public string? Image { get; set; }

		[Required]
		public decimal? Price { get; set; }

		public decimal? CostPrice { get; set; }

		public int? ProductId { get; set; }

		[Required]
		public List<int>? OptionsId { get; set; }
	}
}
