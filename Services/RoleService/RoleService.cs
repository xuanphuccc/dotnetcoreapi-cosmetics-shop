using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly CosmeticsShopContext _context;
        public RoleService(CosmeticsShopContext context)
        {
            _context = context;
        }

        // Get
        public async Task<List<Role>> GetAllRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return roles;
        }

        public async Task<Role> GetRoleById(int roleId)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
            return role!;
        }

        public async Task<Role> GetRoleByName(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            return role!;
        }

        public async Task<List<Role>> GetAdminRoles(AdminUser adminUser)
        {
            var adminRoles = await (from adminRole in _context.AdminRoles
                                    join role in _context.Roles on adminRole.RoleId equals role.RoleId
                                    where adminRole.AdminUserId == adminUser.AdminUserId
                                    select role).ToListAsync();
            return adminRoles;
        }

        // Add
        public async Task<Role> AddRole(Role role)
        {
            await _context.Roles.AddAsync(role);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                return null!;
            }

            return role;
        }

        public async Task<AdminRole> AddAdminRole(AdminRole adminRole)
        {
            await _context.AdminRoles.AddAsync(adminRole);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                return null!;
            }

            return adminRole;
        }

        // Delete
        public async Task<Role> RemoveRole(Role role)
        {
            _context.Roles.Remove(role);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                return null!;
            }

            return role;
        }

        public async Task<AdminRole> RemoveAdminRole(AdminUser adminUser, Role role)
        {
            var existAdminRole = await _context.AdminRoles
                                    .FirstOrDefaultAsync(
                                    a => a.AdminUserId == adminUser.AdminUserId &&
                                    a.RoleId == role.RoleId);

            if (existAdminRole == null)
            {
                return null!;
            }

            _context.AdminRoles.Remove(existAdminRole);
            var result = await _context.SaveChangesAsync();
            if(result == 0)
            {
                return null!;
            }

            return existAdminRole;
        }

        // Update
        public async Task<Role> UpdateRole(Role role)
        {
            var existRole = await GetRoleById(role.RoleId);

            if (existRole == null)
            {
                return null!;
            }

            existRole.Name = role.Name;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return null!;
            }

            return existRole;
        }

        // Convert
        public RoleDTO ConvertToRoleDto(Role role)
        {
            return new RoleDTO()
            {
                RoleId = role.RoleId,
                Name = role.Name
            };
        }
    }
}
