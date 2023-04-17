using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Models.DTO
{
    public class UserReviewDTO
    {
        public int ReviewId { get; set; }

        [Required]
        public double RatingValue { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(450)]
        public string? Comment { get; set; }

        public DateTime? CommentDate { get; set; }

        public string? UserId { get; set; }
        public AppUserDTO? User { get; set; }

        [Required]
        public int? OrderItemId { get; set; }
    }
}
