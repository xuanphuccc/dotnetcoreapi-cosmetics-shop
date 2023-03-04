using System.ComponentModel.DataAnnotations;

namespace asp_dotnet_core_web_api_cosmetics_shop.Models.Entities
{
    public class PaymentType
    {
        [Key]
        public int PaymentTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Value { get; set; }

        public List<PaymentMethod>? PaymentMethods { get; set; }
    }
}
