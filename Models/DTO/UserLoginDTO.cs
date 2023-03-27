using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
    public class UserLoginDTO
    {
        [StringLength(50)]
        [Required]
        public string UserName { get; set; } = string.Empty;

        [StringLength(50)]
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
