﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Provider { get; set; }

        [Required]
        [StringLength(50)]
        public string? AccountNumber { get; set; }

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
