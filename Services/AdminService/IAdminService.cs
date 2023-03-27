using System.Security.Claims;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.AdminService
{
    public interface IAdminService
    {
        // Login, register
        Task<AdminUser> Login(AdminUser adminUser, string password);
        Task<AdminUser> Register(AdminUser adminUser, string password);

        // Get
        AdminUser GetCurrentAdmin(ClaimsPrincipal identityUser);
        Task<AdminUser> GetAdminByUserName(string userName);
        Task<AdminUser> GetAdminByEmail(string email);
        Task<AdminUser> GetAdminById(string userId);
        Task<List<Role>> GetAdminRoles(AdminUser adminUser);
        Task<List<AdminUser>> GetAllAdmins();

        // Generate
        string GenerateToken(AdminUser adminUser, List<Role>? roles = null);
    }
}
