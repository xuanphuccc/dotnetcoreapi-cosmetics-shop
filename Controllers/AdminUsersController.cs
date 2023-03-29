using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.AdminService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminUsersController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [NonAction]
        private AdminUserDTO ConvertToAdminUserDto(AdminUser adminUser)
        {
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
                CreatedAt = adminUser.CreatedAt
            };
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAdminUsers()
        {
            var adminUsers = await _adminService.GetAllAdmins();
            List<AdminUserDTO> adminUsersList = new List<AdminUserDTO>();
            foreach (var item in adminUsers)
            {
                adminUsersList.Add(ConvertToAdminUserDto(item));
            }

            return Ok(new ResponseDTO()
            {
                Data = adminUsersList
            });
        }

        [HttpGet("account")]
        [Authorize]
        public async Task<IActionResult> GetAdminUser()
        {
            var currentIdentityAdmin = _adminService.GetCurrentAdmin(HttpContext.User);
            if (currentIdentityAdmin == null)
            {
                return NotFound();
            }

            var currentAdmin = await _adminService.GetAdminByUserName(currentIdentityAdmin.UserName);
            if (currentAdmin == null)
            {
                return NotFound();
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToAdminUserDto(currentAdmin),
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

            var existEmail = await _adminService.GetAdminByEmail(adminUserDto.Email);
            if (existEmail != null)
            {
                return BadRequest(new ErrorDTO() { Title = "email already exist" });
            }

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
                    CreatedAt = DateTime.Now,
                };

                var createdAdmin = await _adminService.Register(newAdmin, adminUserDto.Password);
                if (createdAdmin == null)
                {
                    return StatusCode(
                                StatusCodes.Status500InternalServerError,
                                new ErrorDTO() { Title = "can not register admin", Status = 500 });
                }


                var loggedAdmin = await _adminService.Login(createdAdmin, adminUserDto.Password);
                if (loggedAdmin == null)
                {
                    return BadRequest(new ErrorDTO() { Title = "invalid username/password", Status = 400 });
                }

                string token = _adminService.GenerateToken(loggedAdmin);

                return Ok(new ResponseDTO()
                {
                    Data = token,
                    Expired = DateTime.Now.AddHours(1)
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

                var loggedAdmin = await _adminService.Login(existAdmin, userLoginDto.Password);
                if (loggedAdmin == null)
                {
                    return BadRequest(new ErrorDTO() { Title = "invalid username/password", Status = 400 });
                }

                var adminRoles = await _adminService.GetAdminRoles(loggedAdmin);

                string token = _adminService.GenerateToken(loggedAdmin, adminRoles);

                return Ok(new ResponseDTO()
                {
                    Data = token,
                    Expired = DateTime.Now.AddHours(1)
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

            var currentIdentityAdmin = _adminService.GetCurrentAdmin(HttpContext.User);
            if (currentIdentityAdmin == null)
            {
                return NotFound();
            }

            var currentAdmin = await _adminService.GetAdminByUserName(currentIdentityAdmin.UserName);
            if (currentAdmin == null)
            {
                return NotFound();
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

                if(currentAdmin.Email != updateAdmin.Email ||
                    currentAdmin.PhoneNumber != updateAdmin.PhoneNumber ||
                    currentAdmin.FullName != updateAdmin.FullName ||
                    currentAdmin.Avatar != updateAdmin.Avatar ||
                    currentAdmin.Bio != updateAdmin.Bio ||
                    currentAdmin.Gender != updateAdmin.Gender ||
                    currentAdmin.BirthDate != updateAdmin.BirthDate)
                {
                    var updatedAdmin = await _adminService.UpdateAdmin(updateAdmin);
                    if (updatedAdmin == null)
                    {
                        return StatusCode(
                                    StatusCodes.Status500InternalServerError,
                                    new ErrorDTO() { Title = "can not update admin", Status = 500 });
                    }

                    return Ok(new ResponseDTO()
                    {
                        Data = ConvertToAdminUserDto(updatedAdmin)
                    });
                } else
                {
                    return StatusCode(
                        StatusCodes.Status304NotModified, 
                        new ErrorDTO() { Title = "not modified", Status = 304 });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }
        }
    }
}
