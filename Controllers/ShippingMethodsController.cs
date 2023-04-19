using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetAllShippingmethods(
            [FromQuery] string? search,
            [FromQuery] string? sort)
        {
            var shippingMethodQuery = _shippingMethodService.FilterAllShippingMethods();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                shippingMethodQuery = _shippingMethodService.FilterSearch(shippingMethodQuery, search);
            }

            // Sort
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "creationtimedesc":
                        shippingMethodQuery = _shippingMethodService.FilterSortByCreationTime(shippingMethodQuery, isDesc: true);
                        break;
                    case "creationtimeasc":
                        shippingMethodQuery = _shippingMethodService.FilterSortByCreationTime(shippingMethodQuery, isDesc: false);
                        break;
                    case "pricedesc":
                        shippingMethodQuery = _shippingMethodService.FilterSortByPrice(shippingMethodQuery, isDesc: true);
                        break;
                    case "priceasc":
                        shippingMethodQuery = _shippingMethodService.FilterSortByPrice(shippingMethodQuery, isDesc: false);
                        break;
                    case "namedesc":
                        shippingMethodQuery = _shippingMethodService.FilterSortByName(shippingMethodQuery, isDesc: true);
                        break;
                    case "nameasc":
                        shippingMethodQuery = _shippingMethodService.FilterSortByName(shippingMethodQuery, isDesc: false);
                        break;
                    default:
                        break;
                }
            }

            var shippingMethods = await shippingMethodQuery.ToListAsync();

            List<ShippingMethodDTO> shippingMethodDtos = new List<ShippingMethodDTO>();
            foreach (var item in shippingMethods)
            {
                var shippingMethodDto = ConvertToShippingMethodDto(item);

                shippingMethodDtos.Add(shippingMethodDto);
            }

            return Ok(new ResponseDTO()
            {
                Data = shippingMethodDtos
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetShippingmethod([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var shippingMethod = await _shippingMethodService.GetShippingMethod(id.Value);
            if (shippingMethod == null)
            {
                return NotFound(new ErrorDTO() { Title = "shipping method not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToShippingMethodDto(shippingMethod)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddShippingmethod([FromBody] ShippingMethodDTO shippingMethodDto)
        {
            if (shippingMethodDto == null)
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
                    CreateAt = DateTime.UtcNow,
                };

                var createdShippingMethod = await _shippingMethodService.AddShippingMethod(newShippingMethod);

                return CreatedAtAction(
                    nameof(GetShippingmethod),
                    new { id = createdShippingMethod.ShippingMethodId },
                    new ResponseDTO()
                    {
                        Data = ConvertToShippingMethodDto(createdShippingMethod),
                        Status = 201,
                    });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateShippingmethod([FromRoute] int? id, [FromBody] ShippingMethodDTO shippingMethodDto)
        {
            if (!id.HasValue || shippingMethodDto == null)
            {
                return BadRequest();
            }

            var existShippingMethod = await _shippingMethodService.GetShippingMethod(id.Value);
            if (existShippingMethod == null)
            {
                return NotFound(new ErrorDTO() { Title = "shipping method not found", Status = 404 });
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

                    return Ok(new ResponseDTO()
                    {
                        Data = ConvertToShippingMethodDto(updateResult),
                    });
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToShippingMethodDto(existShippingMethod),
                Status = 304,
                Title = "not modified",
            });
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemoveShippingmethod([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var existShippingMethod = await _shippingMethodService.GetShippingMethod(id.Value);
            if (existShippingMethod == null)
            {
                return NotFound(new ErrorDTO() { Title = "shipping method not found", Status = 404 });
            }

            // Remove shipping method
            try
            {
                await _shippingMethodService.RemoveShippingMethod(existShippingMethod);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToShippingMethodDto(existShippingMethod)
            });
        }
    }
}
