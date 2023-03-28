using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using web_api_cosmetics_shop.Data;
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
                return null!;
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
                return null!;
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

        public async Task<List<Role>> GetAdminRoles(AdminUser adminUser)
        {
            var adminRoles = await (from adminRole in _context.AdminRoles
                                    join role in _context.Roles on adminRole.RoleId equals role.RoleId
                                    where adminRole.AdminUserId == adminUser.AdminUserId
                                    select role).ToListAsync();
            return adminRoles;
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
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            // Return token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
