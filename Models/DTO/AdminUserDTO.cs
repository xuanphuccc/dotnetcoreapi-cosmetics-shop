﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace web_api_cosmetics_shop.Models.DTO
{
    public class AdminUserDTO
    {
        public string? AdminUserId { get; set; }

        [StringLength(50)]
        [Required]
        public string UserName { get; set; } = string.Empty;

        [StringLength(50)]
        [Required]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        [Required]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Avatar { get; set; }

        [StringLength(200)]
        public string? Bio { get; set; }

        public int? Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<RoleDTO>? Roles { get; set; }
    }
}
