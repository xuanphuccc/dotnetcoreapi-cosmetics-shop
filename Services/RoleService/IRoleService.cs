using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.RoleService
{
    public interface IRoleService
    {
        // Get
        Task<List<Role>> GetAllRoles();
        Task<Role> GetRoleById(int roleId);
        Task<Role> GetRoleByName(string roleName);
        Task<List<Role>> GetAdminRoles(AdminUser adminUser);

        // Add
        Task<Role> AddRole(Role role);

        Task<AdminRole> AddAdminRole(AdminRole adminRole);

        // Update
        Task<Role> UpdateRole(Role role);

        // Delete
        Task<Role> RemoveRole(Role role);

        Task<AdminRole> RemoveAdminRole(AdminUser adminUser, Role role);

        // Convert
        RoleDTO ConvertToRoleDto(Role role);
    }
}
