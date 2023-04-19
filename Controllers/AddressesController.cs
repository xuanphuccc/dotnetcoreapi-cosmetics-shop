using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.AddressService;
using web_api_cosmetics_shop.Services.UserService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        public AddressesController(IAddressService addressService, IUserService userService)
        {
            _addressService = addressService;
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            var allAddresses = await _addressService.GetAllAddresses();

            List<AddressDTO> addresses = new();
            foreach (var item in allAddresses)
            {
                addresses.Add(_addressService.ConvertToAddressDto(item));
            }

            return Ok(new ResponseDTO()
            {
                Data = addresses
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetUserAddresses([FromRoute] string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var userAddress = await _addressService.GetUserAddresses(id);

            List<AddressDTO> listAddress = new();
            foreach (var item in userAddress)
            {
                listAddress.Add(_addressService.ConvertToAddressDto(item));
            }

            return Ok(new ResponseDTO()
            {
                Data = listAddress
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAddress([FromBody] AddressDTO addressDto)
        {
            if (addressDto == null)
            {
                return BadRequest();
            }

            // ** need to double check the user's id
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

            // Add new address
            try
            {
                var newAddress = new Address()
                {
                    UserId = currentUser.UserId,
                    FullName = addressDto.FullName,
                    City = addressDto.City,
                    District = addressDto.District,
                    Ward = addressDto.Ward,
                    AddressLine = addressDto.AddressLine,
                    PhoneNumber = addressDto.PhoneNumber,
                    IsDefault = addressDto.IsDefault,
                    CreateAt = DateTime.UtcNow,
                };

                var addResult = await _addressService.AddAddress(newAddress);

                return CreatedAtAction(nameof(GetUserAddresses),
                                    new { id = addResult.UserId },
                                    new ResponseDTO()
                                    {
                                        Data = _addressService.ConvertToAddressDto(addResult),
                                        Status = 201,
                                        Title = "created",
                                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }
        }

        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateAddress([FromRoute] int? id, [FromBody] AddressDTO addressDto)
        {
            if (!id.HasValue || addressDto == null)
            {
                return BadRequest();
            }

            var existAddress = await _addressService.GetAddress(id.Value);
            if (existAddress == null)
            {
                return NotFound(new ErrorDTO() { Title = "address not found", Status = 404 });
            }

            // ** need to double check the user's id
            // Update address
            try
            {
                var updateAddress = new Address()
                {
                    AddressId = existAddress.AddressId,
                    FullName = addressDto.FullName,
                    City = addressDto.City,
                    District = addressDto.District,
                    Ward = addressDto.Ward,
                    AddressLine = addressDto.AddressLine,
                    PhoneNumber = addressDto.PhoneNumber,
                    IsDefault = addressDto.IsDefault,
                };

                if (existAddress.FullName != addressDto.FullName ||
                    existAddress.City != addressDto.City ||
                    existAddress.District != addressDto.District ||
                    existAddress.Ward != addressDto.Ward ||
                    existAddress.AddressLine != addressDto.AddressLine ||
                    existAddress.PhoneNumber != addressDto.PhoneNumber ||
                    existAddress.IsDefault != addressDto.IsDefault)
                {
                    var updatedAdress = await _addressService.UpdateAddress(updateAddress);

                    return Ok(new ResponseDTO()
                    {
                        Data = _addressService.ConvertToAddressDto(updatedAdress)
                    });
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _addressService.ConvertToAddressDto(existAddress),
                Status = 304,
                Title = "not modified",
            });
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemoveAddress([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var existAddress = await _addressService.GetAddress(id.Value);
            if (existAddress == null)
            {
                return NotFound(new ErrorDTO() { Title = "address not found", Status = 404 });
            }

            // Remove address
            try
            {
                var isHasOrder = await _addressService.IsHasOrder(existAddress);

                if (isHasOrder)
                {
                    // Hide address
                    existAddress.IsDisplay = false;
                    await _addressService.UpdateAddress(existAddress);
                }
                else
                {
                    // Remove address
                    await _addressService.RemoveAddress(existAddress);
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _addressService.ConvertToAddressDto(existAddress),
            });
        }
    }
}
