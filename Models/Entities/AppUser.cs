using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class AppUser
    {
        [Key]
        public string UserId { get; set; } = string.Empty;

        [StringLength(50)]
        [Required]
        public string UserName { get; set; } = string.Empty;

        [StringLength(50)]
        [Required]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        [Required]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Column(TypeName = "ntext")]
        public string? Avatar { get; set; }

        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<Address>? Address { get; set; }
        public List<PaymentMethod>? PaymentMethods { get; set; }
        public List<ShoppingCart>? ShoppingCarts { get; set; }
        public List<UserReview>? UserReviews { get; set; }
        public List<Wishlist>? Wishlists { get; set; }
        public List<ShopOrder>? ShopOrders { get; set; }
    }
}
