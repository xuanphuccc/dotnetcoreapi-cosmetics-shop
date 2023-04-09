using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "ntext")]
        public string? Description { get; set; }

        [Required]
        [Range(1, 100)]
        public int DiscountRate { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        DateTime? CreateAt { get; set; }

        // Collection navigation
        public List<Category>? Categories { get; set; }
    }
}
