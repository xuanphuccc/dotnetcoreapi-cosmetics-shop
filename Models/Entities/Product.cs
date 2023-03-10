using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(450)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        public string? Image { get; set; }

        [Required]
        public bool IsDisplay { get; set; }


        public List<ProductItem>? ProductItems { get; set; }

        public List<ProductCategory>? ProductCategories { get; set; }
    }
}
