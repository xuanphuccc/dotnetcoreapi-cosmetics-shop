using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
    public class RoleDTO
    {
        public int RoleId { get; set; }

        [StringLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
