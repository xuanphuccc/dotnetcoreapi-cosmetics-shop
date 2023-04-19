using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.AdminService;
using web_api_cosmetics_shop.Services.RoleService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IRoleService _roleService;

        public AdminUsersController(IAdminService adminService, IRoleService roleService)
        {
            _adminService = adminService;
            _roleService = roleService;
        }

        [NonAction]
        private async Task<AdminUserDTO> ConvertToAdminUserDto(AdminUser adminUser)
        {
            var adminRoles = await _roleService.GetAdminRoles(adminUser);

            List<RoleDTO> adminRolesDto = new();
            foreach (var item in adminRoles)
            {
                adminRolesDto.Add(_roleService.ConvertToRoleDto(item));
            }

            return new AdminUserDTO()
            {
                AdminUserId = adminUser.AdminUserId,
                UserName = adminUser.UserName,
                Email = adminUser.Email,
                PhoneNumber = adminUser.PhoneNumber,
                FullName = adminUser.FullName,
                Avatar = adminUser.Avatar,
                Bio = adminUser.Bio,
                Gender = adminUser.Gender,
                BirthDate = adminUser.BirthDate,
                CreatedAt = adminUser.CreatedAt,
                Roles = adminRolesDto
            };
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAdminUsers()
        {
            var adminUsers = await _adminService.GetAllAdmins();
            List<AdminUserDTO> adminUsersList = new List<AdminUserDTO>();
            foreach (var item in adminUsers)
            {
                adminUsersList.Add(await ConvertToAdminUserDto(item));
            }

            return Ok(new ResponseDTO()
            {
                Data = adminUsersList
            });
        }

        // Get current logged admin user (by access token)
        [HttpGet("account")]
        [Authorize]
        public async Task<IActionResult> GetCurrentAdminUser()
        {
            // Get admin from token
            var currentIdentityAdmin = _adminService.GetCurrentAdmin(HttpContext.User);
            if (currentIdentityAdmin == null)
            {
                return NotFound(new ErrorDTO() { Title = "admin not found", Status = 404 });
            }

            // Get admin from database
            var currentAdmin = await _adminService.GetAdminByUserName(currentIdentityAdmin.UserName);
            if (currentAdmin == null)
            {
                return NotFound(new ErrorDTO() { Title = "admin not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToAdminUserDto(currentAdmin),
            });
        }

        // Get other admin user (by id)
        [HttpGet("account/{id?}")]
        [Authorize]
        public async Task<IActionResult> GetAdminUser([FromRoute] string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var existAdmin = await _adminService.GetAdminById(id);
            if (existAdmin == null)
            {
                return NotFound(new ErrorDTO() { Title = "admin not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToAdminUserDto(existAdmin),
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminUserDTO adminUserDto)
        {
            if (adminUserDto == null)
            {
                return BadRequest();
            }

            // Check exist email
            var existEmail = await _adminService.GetAdminByEmail(adminUserDto.Email);
            if (existEmail != null)
            {
                return BadRequest(new ErrorDTO() { Title = "email already exist" });
            }

            // Check exist username
            var existUserName = await _adminService.GetAdminByUserName(adminUserDto.UserName);
            if (existUserName != null)
            {
                return BadRequest(new ErrorDTO() { Title = "username already exist" });
            }

            try
            {
                var newAdmin = new AdminUser()
                {
                    AdminUserId = Guid.NewGuid().ToString(),
                    UserName = adminUserDto.UserName,
                    Email = adminUserDto.Email,
                    PhoneNumber = adminUserDto.PhoneNumber,
                    FullName = adminUserDto.FullName,
                    Avatar = adminUserDto.Avatar,
                    Gender = adminUserDto.Gender,
                    BirthDate = adminUserDto.BirthDate,
                    CreatedAt = DateTime.UtcNow,
                };

                // Register admin
                var createdAdmin = await _adminService.Register(newAdmin, adminUserDto.Password);

                // Login admin
                var loggedAdmin = await _adminService.Login(createdAdmin, adminUserDto.Password);

                // Generate token
                string token = _adminService.GenerateToken(loggedAdmin);

                return Ok(new ResponseDTO()
                {
                    Data = token,
                    Expired = DateTime.Now.AddHours(4)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAdmin(UserLoginDTO userLoginDto)
        {
            if (userLoginDto == null)
            {
                return BadRequest();
            }

            try
            {
                var existAdmin = await _adminService.GetAdminByUserName(userLoginDto.UserName);
                if (existAdmin == null)
                {
                    return BadRequest(new ErrorDTO() { Title = "invalid username/password", Status = 400 });
                }

                // Login admin
                var loggedAdmin = await _adminService.Login(existAdmin, userLoginDto.Password);

                // Get admin roles
                var adminRoles = await _roleService.GetAdminRoles(loggedAdmin);

                // Generate token
                string token = _adminService.GenerateToken(loggedAdmin, adminRoles);

                return Ok(new ResponseDTO()
                {
                    Data = token,
                    Expired = DateTime.Now.AddHours(4)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }
        }

        [HttpPut("account")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] AdminUserDTO adminUserDto)
        {
            if (adminUserDto == null)
            {
                return BadRequest();
            }

            // Get admin from token
            var currentIdentityAdmin = _adminService.GetCurrentAdmin(HttpContext.User);
            if (currentIdentityAdmin == null)
            {
                return NotFound(new ErrorDTO() { Title = "admin not found", Status = 404 });
            }

            // Get admin from database
            var currentAdmin = await _adminService.GetAdminByUserName(currentIdentityAdmin.UserName);
            if (currentAdmin == null)
            {
                return NotFound(new ErrorDTO() { Title = "admin not found", Status = 404 });
            }

            // Check exist email
            var existEmail = await _adminService.GetAdminByEmail(adminUserDto.Email);
            if (existEmail != null && currentAdmin.Email.ToLower() != adminUserDto.Email.ToLower())
            {
                return BadRequest(new ErrorDTO() { Title = "email already exist" });
            }

            try
            {
                var updateAdmin = new AdminUser()
                {
                    UserName = currentAdmin.UserName,
                    Email = adminUserDto.Email,
                    PhoneNumber = adminUserDto.PhoneNumber,
                    FullName = adminUserDto.FullName,
                    Avatar = adminUserDto.Avatar,
                    Bio = adminUserDto.Bio,
                    Gender = adminUserDto.Gender,
                    BirthDate = adminUserDto.BirthDate,
                };

                if (currentAdmin.Email != updateAdmin.Email ||
                    currentAdmin.PhoneNumber != updateAdmin.PhoneNumber ||
                    currentAdmin.FullName != updateAdmin.FullName ||
                    currentAdmin.Avatar != updateAdmin.Avatar ||
                    currentAdmin.Bio != updateAdmin.Bio ||
                    currentAdmin.Gender != updateAdmin.Gender ||
                    currentAdmin.BirthDate != updateAdmin.BirthDate)
                {
                    var updatedAdmin = await _adminService.UpdateAdmin(updateAdmin);

                    return Ok(new ResponseDTO()
                    {
                        Data = await ConvertToAdminUserDto(updatedAdmin)
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToAdminUserDto(currentAdmin),
                Status = 304,
                Title = "not modified",
            });
        }

        [HttpPut("account/{id?}")]
        [Authorize]
        public async Task<IActionResult> UpdateAdminUserRole(string? id, AdminUserDTO adminUserDto)
        {
            if (string.IsNullOrEmpty(id) || adminUserDto == null)
            {
                return BadRequest();
            }

            var existAdmin = await _adminService.GetAdminById(id);
            if (existAdmin == null)
            {
                return NotFound(new ErrorDTO() { Title = "admin not found", Status = 404 });
            }

            var existAdminRoles = await _roleService.GetAdminRoles(existAdmin);
            var existAdminRolesId = existAdminRoles.Select(r => r.RoleId).ToList();

            var newAdminRoles = adminUserDto.Roles ?? new List<RoleDTO>();
            var newAdminRolesId = newAdminRoles.Select(r => r.RoleId).ToList();

            try
            {
                // Delete list: in existAdminRoles but not in newAdminRoles
                foreach (var item in existAdminRoles)
                {
                    if (!newAdminRolesId.Contains(item.RoleId))
                    {
                        await _roleService.RemoveAdminRole(existAdmin, item);
                    }
                }

                // Add list: in newAdminRoles but not in existAdminRoles
                foreach (var item in newAdminRoles)
                {
                    if (!existAdminRolesId.Contains(item.RoleId))
                    {
                        var newAdminRole = new AdminRole()
                        {
                            AdminUserId = existAdmin.AdminUserId,
                            RoleId = item.RoleId,
                        };

                        await _roleService.AddAdminRole(newAdminRole);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = adminUserDto
            });
        }
    }
}
