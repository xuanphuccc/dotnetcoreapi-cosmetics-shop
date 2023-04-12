using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class UserReview
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public double RatingValue { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(450)]
        public string? Comment { get; set; }

        [Required]
        public DateTime CommentDate { get; set; }


        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }

        public int? OrderItemId { get; set; }
        [ForeignKey("OrderItemId")]
        public OrderItem? OrderItem { get; set; }
    }
}
