using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodId { get; set; }

        [Required]
        [StringLength(100)]
        public string Provider { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CardholderName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string SecurityCode { get; set; } = string.Empty;

        [StringLength(50)]
        public string? PostalCode { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public bool IsDefault { get; set; }


        public int? PaymentTypeId { get; set; }
        [ForeignKey("PaymentTypeId")]
        public PaymentType? PaymentType { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? AppUser { get; set; }


        public List<ShopOrder>? ShopOrders { get; set; }
    }
}
