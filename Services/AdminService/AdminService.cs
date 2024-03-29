﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly CosmeticsShopContext _context;
        private readonly IConfiguration _configuration;
        public AdminService(CosmeticsShopContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Login and Register
        public async Task<AdminUser> Login(AdminUser adminUser, string password)
        {
            var existAdminUser = await _context.AdminUsers
                            .FirstOrDefaultAsync(
                                au => au.UserName.ToLower() == adminUser.UserName.ToLower() && au.Password == password);

            if (existAdminUser == null)
            {
                throw new Exception("invalid username/password");
            }

            return existAdminUser;
        }

        public async Task<AdminUser> Register(AdminUser adminUser, string password)
        {
            adminUser.Password = password;

            await _context.AdminUsers.AddAsync(adminUser);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("can not register admin");
            }

            return adminUser;
        }

        // Get
        public async Task<List<AdminUser>> GetAllAdmins()
        {
            var adminUsers = await _context.AdminUsers.ToListAsync();
            return adminUsers;
        }

        public AdminUser GetCurrentAdmin(ClaimsPrincipal identityUser)
        {
            var identity = identityUser.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return null!;
            }

            var userClaims = identity.Claims;

            return new AdminUser
            {
                UserName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value!,
                Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value!,
                FullName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value!,
            };
        }

        public async Task<AdminUser> GetAdminById(string userId)
        {
            var adminUser = await _context.AdminUsers.FirstOrDefaultAsync(au => au.AdminUserId == userId);
            if (adminUser == null)
            {
                return null!;
            }

            return adminUser;
        }

        public async Task<AdminUser> GetAdminByUserName(string userName)
        {
            var adminUser = await _context.AdminUsers.FirstOrDefaultAsync(au => au.UserName.ToLower() == userName.ToLower());
            if (adminUser == null)
            {
                return null!;
            }

            return adminUser;
        }

        public async Task<AdminUser> GetAdminByEmail(string email)
        {
            var adminUser = await _context.AdminUsers.FirstOrDefaultAsync(au => au.Email.ToLower() == email.ToLower());
            if (adminUser == null)
            {
                return null!;
            }

            return adminUser;
        }

        // Update
        public async Task<AdminUser> UpdateAdmin(AdminUser adminUser)
        {
            var existAdminUser = await GetAdminByUserName(adminUser.UserName);
            if (existAdminUser == null)
            {
                throw new Exception("admin not found");
            }

            existAdminUser.Email = adminUser.Email;
            existAdminUser.PhoneNumber = adminUser.PhoneNumber;
            existAdminUser.FullName = adminUser.FullName;
            existAdminUser.Avatar = adminUser.Avatar;
            existAdminUser.Bio = adminUser.Bio;
            existAdminUser.Gender = adminUser.Gender;
            existAdminUser.BirthDate = adminUser.BirthDate;

            var result = await _context.SaveChangesAsync();
            if(result == 0)
            {
                throw new Exception("can not update admin");
            }

            return existAdminUser;
        }

        // Generate token
        public string GenerateToken(AdminUser adminUser, List<Role>? roles = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Attach payload
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, adminUser.UserName),
                new Claim(ClaimTypes.Email, adminUser.Email),
                new Claim(ClaimTypes.Name, adminUser.FullName),
            };

            if (roles != null)
            {
                // Add roles
                claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));
            }

            // Create token
            var token = new JwtSecurityToken(
              _configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddHours(4),
              signingCredentials: credentials);

            // Return token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
