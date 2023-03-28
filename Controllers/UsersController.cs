using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.UserService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [NonAction]
        private AppUserDTO ConvertToAppUserDto(AppUser appUser)
        {
            return new AppUserDTO()
            {
                UserId = appUser.UserId,
                UserName = appUser.UserName,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                FullName = appUser.FullName,
                Avatar = appUser.Avatar,
                Gender = appUser.Gender,
                BirthDate = appUser.BirthDate,
                CreatedAt = appUser.CreatedAt
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var allUsers = await _userService.GetAllUsers();

            List<AppUserDTO> users = new List<AppUserDTO>();
            foreach (var user in allUsers)
            {
                users.Add(ConvertToAppUserDto(user));
            }

            return Ok(new ResponseDTO()
            {
                Data = users
            });
        }

        [HttpGet("account")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdentity = _userService.GetCurrentUser(HttpContext.User);
            if (userIdentity == null)
            {
                return NotFound();
            }

            var currentUser = await _userService.GetUserByUserName(userIdentity.UserName);
            if (currentUser == null)
            {
                return NotFound();
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToAppUserDto(currentUser)
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] AppUserDTO appUserDto)
        {
            if (appUserDto == null)
            {
                return BadRequest();
            }

            var existEmail = await _userService.GetUserByEmail(appUserDto.Email);
            if (existEmail != null)
            {
                return BadRequest(new ErrorDTO() { Title = "email already exist" });
            }

            var existUsername = await _userService.GetUserByUserName(appUserDto.UserName);
            if (existUsername != null)
            {
                return BadRequest(new ErrorDTO() { Title = "username already exist" });
            }

            try
            {
                var newUser = new AppUser()
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = appUserDto.UserName,
                    Email = appUserDto.Email,
                    PhoneNumber = appUserDto.PhoneNumber,
                    FullName = appUserDto.FullName,
                    Avatar = appUserDto.Avatar,
                    Gender = appUserDto.Gender,
                    BirthDate = appUserDto.BirthDate,
                    CreatedAt = DateTime.Now,
                };

                var registedUser = await _userService.Register(newUser, appUserDto.Password);
                if (registedUser == null)
                {
                    return StatusCode(
                                StatusCodes.Status500InternalServerError,
                                new ErrorDTO() { Title = "can not register user", Status = 500 });
                }

                var loggedUser = await _userService.Login(registedUser, registedUser.Password);
                if(loggedUser == null)
                {
                    return BadRequest(new ErrorDTO() { Title = "invalid username/password", Status = 400 });
                }

                var token = _userService.GenerateToken(loggedUser);

                return Ok(new ResponseDTO()
                {
                    Data = token,
                    Expired = DateTime.Now.AddMinutes(15),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserLoginDTO userLoginDto)
        {
            if(userLoginDto == null)
            {
                return BadRequest();
            }

            try
            {
                var existUser = await _userService.GetUserByUserName(userLoginDto.UserName);
                if(existUser == null)
                {
                    return BadRequest(new ErrorDTO() { Title = "invalid username/password", Status = 400 });
                }

                var loggedUser = await _userService.Login(existUser, userLoginDto.Password);
                if(loggedUser == null)
                {
                    return BadRequest(new ErrorDTO() { Title = "invalid username/password", Status = 400 });
                }

                var token = _userService.GenerateToken(loggedUser);

                return Ok(new ResponseDTO()
                {
                    Data = token,
                    Expired = DateTime.Now.AddMinutes(15)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }
        }
    }
}
