using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class AppUser
    {
        // Xóa bỏ sau khi kế thừa IdentityUser
        [Key]
        public string? UserId { get; set; }

        public List<Address>? Address { get; set; }
        public List<PaymentMethod>? PaymentMethods { get; set; }
        public List<ShoppingCart>? ShoppingCarts { get; set; }
        public List<UserReview>? UserReviews { get; set; }
    }
}
