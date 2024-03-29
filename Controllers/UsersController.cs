﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.AddressService;
using web_api_cosmetics_shop.Services.PaymentMethodService;
using web_api_cosmetics_shop.Services.UserService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly IPaymentMethodService _paymentMethodService;
        public UsersController(IUserService userService, IAddressService addressService, IPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
            _addressService = addressService;
            _userService = userService;
        }

        [NonAction]
        private async Task<AppUserDTO> ConvertToAppUserDto(AppUser appUser)
        {
            //Get User Adresses
            var userAddresses = await _addressService.GetUserAddresses(appUser.UserId);

            List<AddressDTO> addressesDtos = new List<AddressDTO>();
            foreach (var address in userAddresses)
            {
                var addressDto = _addressService.ConvertToAddressDto(address);
                addressesDtos.Add(addressDto);
            }

            //Get user payment methods
            var userPaymentMethods = await _paymentMethodService.GetUserPaymentMethods(appUser.UserId);

            List<PaymentMethodDTO> paymentMethodDtos = new List<PaymentMethodDTO>();
            foreach (var paymentMethod in userPaymentMethods)
            {
                var paymentMethodDto = _paymentMethodService.ConvertToPaymentMethodDto(paymentMethod);
                paymentMethodDtos.Add(paymentMethodDto);
            }

            var appUserDto = _userService.ConvertToAppUserDto(appUser);
            appUserDto.Addresses = addressesDtos;
            appUserDto.PaymentMethods = paymentMethodDtos;

            return appUserDto;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var allUsers = await _userService.GetAllUsers();

            List<AppUserDTO> users = new List<AppUserDTO>();
            foreach (var user in allUsers)
            {
                users.Add(await ConvertToAppUserDto(user));
            }

            return Ok(new ResponseDTO()
            {
                Data = users
            });
        }

        // Get current logged user (by access token)
        [HttpGet("account")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Get user from token
            var userIdentity = _userService.GetCurrentUser(HttpContext.User);
            if (userIdentity == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            // Get user from database
            var currentUser = await _userService.GetUserByUserName(userIdentity.UserName);
            if (currentUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToAppUserDto(currentUser)
            });
        }

        // Get other user (by id)
        [HttpGet("account/{id?}")]
        [Authorize]
        public async Task<IActionResult> GetUser([FromRoute] string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var existUser = await _userService.GetUserById(id);
            if (existUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToAppUserDto(existUser),
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] AppUserDTO appUserDto)
        {
            if (appUserDto == null)
            {
                return BadRequest();
            }

            // Check exist email
            var existEmail = await _userService.GetUserByEmail(appUserDto.Email);
            if (existEmail != null)
            {
                return BadRequest(new ErrorDTO() { Title = "email already exist" });
            }

            // Check exist username
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
                    CreatedAt = DateTime.UtcNow,
                };

                // Register user
                var registedUser = await _userService.Register(newUser, appUserDto.Password);

                // Login user
                var loggedUser = await _userService.Login(registedUser, registedUser.Password);

                // Generate token
                var token = _userService.GenerateToken(loggedUser);

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
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(UserLoginDTO userLoginDto)
        {
            if (userLoginDto == null)
            {
                return BadRequest();
            }

            try
            {
                var existUser = await _userService.GetUserByUserName(userLoginDto.UserName);
                if (existUser == null)
                {
                    return BadRequest(new ErrorDTO() { Title = "invalid username/password", Status = 400 });
                }

                // Login admin
                var loggedUser = await _userService.Login(existUser, userLoginDto.Password);

                // Generate token
                var token = _userService.GenerateToken(loggedUser);

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
        public async Task<IActionResult> UpdateProfile([FromBody] AppUserDTO appUserDto)
        {
            if (appUserDto == null)
            {
                return BadRequest();
            }

            // Get user from token
            var currentIdentityUser = _userService.GetCurrentUser(HttpContext.User);
            if (currentIdentityUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            // Get user from database
            var currentUser = await _userService.GetUserByUserName(currentIdentityUser.UserName);
            if (currentUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            // Check exist email
            var existEmail = await _userService.GetUserByEmail(appUserDto.Email);
            if (existEmail != null && appUserDto.Email.ToLower() != currentUser.Email.ToLower())
            {
                return BadRequest(new ErrorDTO() { Title = "email already exist" });
            }

            try
            {
                var updateUser = new AppUser()
                {
                    UserName = currentUser.UserName,
                    Email = appUserDto.Email,
                    PhoneNumber = appUserDto.PhoneNumber,
                    FullName = appUserDto.FullName,
                    Avatar = appUserDto.Avatar,
                    Bio = appUserDto.Bio,
                    Gender = appUserDto.Gender,
                    BirthDate = appUserDto.BirthDate,
                };

                if (currentUser.Email != updateUser.Email ||
                    currentUser.PhoneNumber != updateUser.PhoneNumber ||
                    currentUser.FullName != updateUser.FullName ||
                    currentUser.Avatar != updateUser.Avatar ||
                    currentUser.Bio != updateUser.Bio ||
                    currentUser.Gender != updateUser.Gender ||
                    currentUser.BirthDate != updateUser.BirthDate)
                {
                    // Update user
                    var updatedUser = await _userService.UpdateUser(updateUser);

                    return Ok(new ResponseDTO()
                    {
                        Data = await ConvertToAppUserDto(updatedUser)
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToAppUserDto(currentUser),
                Status = 304,
                Title = "not modified",
            });
        }

    }
}
