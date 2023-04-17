using System.Security.Claims;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.UserService
{
    public interface IUserService
    {
        // Login, register
        Task<AppUser> Login(AppUser appUser, string password);
        Task<AppUser> Register(AppUser appUser, string password);

        // Get
        AppUser GetCurrentUser(ClaimsPrincipal identityUser);
        Task<AppUser> GetUserByUserName(string userName);
        Task<AppUser> GetUserByEmail(string email);
        Task<AppUser> GetUserById(string userId);
        Task<List<AppUser>> GetAllUsers();

        // Update
        Task<AppUser> UpdateUser(AppUser adminUser);

        // Convert to DTO
        AppUserDTO ConvertToAppUserDto(AppUser appUser);

        // Generate token
        string GenerateToken(AppUser appUser);
    }
}
