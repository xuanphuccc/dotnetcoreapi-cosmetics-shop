using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(256)]
        public string? Name { get; set; }

        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}
