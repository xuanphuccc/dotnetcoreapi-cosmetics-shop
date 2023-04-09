using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        [StringLength(50)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string District { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Ward { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string AddressLine { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public bool IsDefault { get; set; }

        public DateTime? CreateAt { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }


        public List<ShopOrder>? ShopOrders { get; set; }
    }
}
