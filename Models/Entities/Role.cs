using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [StringLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;

        public List<AdminRole>? AdminRoles { get; set; }
    }
}
