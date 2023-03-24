using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_api_cosmetics_shop.Models.Entities
{
    public class AdminRole
    {
        [Key]
        public int AdminRoleId { get; set; }

        public int? RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        public string? AdminUserId { get; set; }
        [ForeignKey("AdminUserId")]
        public AdminUser? AdminUser { get; set; }

    }
}
