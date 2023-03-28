using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly CosmeticsShopContext _context;
        private readonly IConfiguration _configuration;
        public UserService(CosmeticsShopContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        // Login, Register
        public async Task<AppUser> Login(AppUser appUser, string password)
        {
            var existUser = await _context.AppUsers
                .FirstOrDefaultAsync(
                u => u.UserName.ToLower() == appUser.UserName.ToLower() && u.Password == password);

            if (existUser == null)
            {
                return null!;
            }

            return existUser;
        }

        public async Task<AppUser> Register(AppUser appUser, string password)
        {
            appUser.Password = password;

            await _context.AppUsers.AddAsync(appUser);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                return null!;
            }

            return appUser;
        }

        // Get
        public async Task<List<AppUser>> GetAllUsers()
        {
            var allUsers = await _context.AppUsers.ToListAsync();
            return allUsers;
        }

        public AppUser GetCurrentUser(ClaimsPrincipal identityUser)
        {
            var identity = identityUser.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return null!;
            }

            var userClaims = identity.Claims;

            return new AppUser()
            {
                UserName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value!,
                Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value!,
                FullName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value!,
            };

        }

        public async Task<AppUser> GetUserById(string userId)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return null!;
            }

            return user;
        }

        public async Task<AppUser> GetUserByUserName(string userName)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return null!;
            }

            return user;
        }

        public async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null!;
            }

            return user;
        }

        // Generate token
        public string GenerateToken(AppUser appUser)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Attach payload
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.UserName),
                new Claim(ClaimTypes.Email, appUser.Email),
                new Claim(ClaimTypes.Name, appUser.FullName),
            };

            // Create token
            var token = new JwtSecurityToken(
              _configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddHours(1),
              signingCredentials: credentials);

            // Return token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
