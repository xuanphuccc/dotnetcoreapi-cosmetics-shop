using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class ShoppingCart
    {
        [Key]
        public int CartId { get; set; }


        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }

        public List<ShoppingCartItem>? Items { get; set; }
    }
}
