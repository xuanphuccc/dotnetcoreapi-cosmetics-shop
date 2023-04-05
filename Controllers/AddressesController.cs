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
			_userService= userService;
		}


		[HttpGet]
		public async Task<IActionResult> GetAllAddresses()
		{
			var allAddresses = await _addressService.GetAllAddresses();

			List<AddressDTO> addresses = new List<AddressDTO>();
            foreach (var item in allAddresses)
            {
				addresses.Add(_addressService.ConvertToAddressDto(item));
            }

			return Ok(addresses);
		}

		[HttpGet("{id?}")]
		public async Task<IActionResult> GetUserAddresses([FromRoute] string? id)
		{
			if(string.IsNullOrEmpty(id))
			{
				return BadRequest();
			}

			var userAddress = await _addressService.GetUserAddresses(id);

			List<AddressDTO> listAddress = new List<AddressDTO>();
            foreach (var item in userAddress)
            {
				listAddress.Add(_addressService.ConvertToAddressDto(item));
            }

            return Ok(listAddress);
		}

		[HttpPost]
		public async Task<IActionResult> AddAddress([FromBody] AddressDTO addressDto)
		{
			if(addressDto == null)
			{
				return BadRequest();
			}

			// ** need to double check the user's id
			var userIdentity = _userService.GetCurrentUser(HttpContext.User);
			if(userIdentity== null)
			{
				return NotFound();
			}
			var currentUser = await _userService.GetUserByUserName(userIdentity.UserName);
			if (currentUser == null)
			{
				return NotFound();
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
				};

				var addResult = await _addressService.AddAddress(newAddress);
				if(addResult == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not create address", Status = 500 });
				}

				return CreatedAtAction(nameof(GetUserAddresses), 
									new {id = addResult.UserId}, 
									_addressService.ConvertToAddressDto(addResult));
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}

		}

		[HttpPut("{id?}")]
		public async Task<IActionResult> UpdateAddress([FromRoute] int? id,[FromBody] AddressDTO addressDto)
		{
			if (!id.HasValue || addressDto == null)
			{
				return BadRequest();
			}

			var existAddress = await _addressService.GetAddress(id.Value);
			if(existAddress == null)
			{
				return NotFound();
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

				if(existAddress.FullName != addressDto.FullName ||
					existAddress.City != addressDto.City ||
					existAddress.District != addressDto.District ||
					existAddress.Ward != addressDto.Ward ||
					existAddress.AddressLine != addressDto.AddressLine ||
					existAddress.PhoneNumber != addressDto.PhoneNumber ||
					existAddress.IsDefault != addressDto.IsDefault)
				{
					var updateResult = await _addressService.UpdateAddress(updateAddress);
					if (updateResult == null)
					{
						return StatusCode(StatusCodes.Status500InternalServerError,
										new ErrorDTO() { Title = "Can not update address", Status = 500 });
					}

				}
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}

			return Ok(addressDto);
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
				return NotFound();
			}

			// Remove address
			try
			{
				var removeResult = await _addressService.RemoveAddress(existAddress);
				if (removeResult == 0)
				{
					return StatusCode(StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not remove address", Status = 500 });
				}
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}

			return Ok(_addressService.ConvertToAddressDto(existAddress));
		}
	}
}
