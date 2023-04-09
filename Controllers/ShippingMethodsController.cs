using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.ShippingMethodService;

namespace web_api_cosmetics_shop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShippingMethodsController : ControllerBase
	{
		private readonly IShippingMethodService _shippingMethodService;

		public ShippingMethodsController(IShippingMethodService shippingMethodService)
		{
			_shippingMethodService = shippingMethodService;
		}

		[NonAction]
		private ShippingMethodDTO ConvertToShippingMethodDto(ShippingMethod shippingMethod)
		{
			var shippingMethodDto = new ShippingMethodDTO()
			{
				ShippingMethodId = shippingMethod.ShippingMethodId,
				Name = shippingMethod.Name,
				Price = shippingMethod.Price,
				CreateAt = shippingMethod.CreateAt,
			};

			return shippingMethodDto;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllShippingmethods()
		{
			var shippingMethods = await _shippingMethodService.GetAllShippingMethods();

			List<ShippingMethodDTO> shippingMethodDtos = new List<ShippingMethodDTO>();
            foreach (var item in shippingMethods)
            {
				var shippingMethodDto = ConvertToShippingMethodDto(item);

				shippingMethodDtos.Add(shippingMethodDto);
            }

            return Ok(shippingMethodDtos);
		}

		[HttpGet("{id?}")]
		public async Task<IActionResult> GetShippingmethod([FromRoute] int? id)
		{
			if(!id.HasValue)
			{
				return BadRequest();
			}

			var shippingMethod = await _shippingMethodService.GetShippingMethod(id.Value);
			if(shippingMethod == null)
			{
				return NotFound();
			}

			var shippingMethodDto = ConvertToShippingMethodDto(shippingMethod);

			return Ok(shippingMethodDto);
		}

		[HttpPost]
		public async Task<IActionResult> AddShippingmethod([FromBody] ShippingMethodDTO shippingMethodDto)
		{
			if(shippingMethodDto == null)
			{
				return BadRequest();
			}

			// Create shipping method
			try
			{
				var newShippingMethod = new ShippingMethod()
				{
					Name = shippingMethodDto.Name,
					Price = shippingMethodDto.Price,
					CreateAt = DateTime.Now,
				};

				var createdShippingMethod = await _shippingMethodService.AddShippingMethod(newShippingMethod);
				if (createdShippingMethod == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not create shipping method", Status = 500 });
				}

				// Get Information
				shippingMethodDto.ShippingMethodId = createdShippingMethod.ShippingMethodId;

				return CreatedAtAction(
					nameof(GetShippingmethod),
					new { id = createdShippingMethod.ShippingMethodId },
					shippingMethodDto);
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}

		[HttpPut("{id?}")]
		public async Task<IActionResult> UpdateShippingmethod([FromRoute] int? id, [FromBody] ShippingMethodDTO shippingMethodDto)
		{
			if(!id.HasValue || shippingMethodDto == null)
			{
				return BadRequest();
			}

			var existShippingMethod = await _shippingMethodService.GetShippingMethod(id.Value);
			if(existShippingMethod == null)
			{
				return NotFound();
			}

			// Update shiping method
			try
			{
				var updateShippingMethod = new ShippingMethod()
				{
					ShippingMethodId = existShippingMethod.ShippingMethodId,
					Name = shippingMethodDto.Name,
					Price = shippingMethodDto.Price
				};

				if (existShippingMethod.Name != updateShippingMethod.Name ||
					existShippingMethod.Price != updateShippingMethod.Price)
				{
					var updateResult = await _shippingMethodService.UpdateShippingMethod(updateShippingMethod);
					if (updateResult == null)
					{
						return StatusCode(StatusCodes.Status500InternalServerError,
										new ErrorDTO() { Title = "Can not update shipping method", Status = 500 });
					}
				}
			}
			catch(Exception error)
			{
				return BadRequest(new ErrorDTO() { Title= error.Message, Status = 400 });
			} 

			return Ok(shippingMethodDto);
		}

		[HttpDelete("{id?}")]
		public async Task<IActionResult> RemoveShippingmethod([FromRoute] int? id)
		{
			if (!id.HasValue)
			{
				return BadRequest();
			}

			var existShippingMethod = await _shippingMethodService.GetShippingMethod(id.Value);

			var hasRemove = ConvertToShippingMethodDto(existShippingMethod);

			if (existShippingMethod == null)
			{
				return NotFound();
			}

			// Remove shipping method
			try
			{
				var removeResult = await _shippingMethodService.RemoveShippingMethod(existShippingMethod);
				if (removeResult == 0)
				{
					return StatusCode(StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not remove shipping method", Status = 500 });
				}
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}

			return Ok(hasRemove);
		}
	}
}
