using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.ProductOptionService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOptionsController : ControllerBase
    {
        private readonly IProductOptionService _productOptionService;
        public ProductOptionsController(IProductOptionService productOptionService)
        {
            _productOptionService = productOptionService;
        }

        [NonAction]
        private async Task<ProductOptionTypeDTO> ConvertToProductOptionTypeDto(ProductOptionType optionsType)
        {
            // Get Options of Type
            var options = await _productOptionService.GetOptions(optionsType);

            // Convert ProductOption to ProductOptionDTO
            List<ProductOptionDTO> productOptionDtos = new List<ProductOptionDTO>();
            foreach (var option in options)
            {
                var productOptionDto = new ProductOptionDTO()
                {
                    ProductOptionId = option.ProductOptionId,
                    OptionTypeId = optionsType.OptionTypeId,
                    Name = option.Name,
                    Value = option.Value
                };

                productOptionDtos.Add(productOptionDto);
            }

            // Convert ProductOptionType to ProductOptionTypeDTO
            var productOptionTypeDto = new ProductOptionTypeDTO()
            {
                OptionTypeId = optionsType.OptionTypeId,
                Name = optionsType.Name,
                Options = productOptionDtos
            };

            return productOptionTypeDto;
        }

        // ----------- GET ALL -----------
        // GET: api/productoptions
        [HttpGet]
        public async Task<IActionResult> GetAllProductOptions(
            [FromQuery] string? search,
            [FromQuery] string? sort)
        {
            var allOptionTypesQuery = _productOptionService.FilterAllProductOptionTypes();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                allOptionTypesQuery = _productOptionService.FilterSearch(allOptionTypesQuery, search);
            }

            // Sort
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "namedesc":
                        allOptionTypesQuery = _productOptionService.FilterSortByName(allOptionTypesQuery, isDesc: true);
                        break;
                    case "nameasc":
                        allOptionTypesQuery = _productOptionService.FilterSortByName(allOptionTypesQuery, isDesc: false);
                        break;
                    default:
                        break;
                }
            }

            var allOptionsType = await allOptionTypesQuery.ToListAsync();

            List<ProductOptionTypeDTO> productOptionTypeDtos = new List<ProductOptionTypeDTO>();
            foreach (var type in allOptionsType)
            {
                var productOptionTypeDto = await ConvertToProductOptionTypeDto(type);
                productOptionTypeDtos.Add(productOptionTypeDto);
            }

            return Ok(new ResponseDTO()
            {
                Data = productOptionTypeDtos
            });
        }

        // ----------- GET -----------
        // GET: api/productoptions/9
        [HttpGet("{id?}")]
        public async Task<IActionResult> GetProductOptions([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get Options Type
            var optionsType = await _productOptionService.GetOptionsTypeById(id.Value);
            if (optionsType == null)
            {
                return NotFound(new ErrorDTO() { Title = "options type not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToProductOptionTypeDto(optionsType)
            });
        }

        // ----------- POST -----------
        // POST: api/productoptions
        [HttpPost]
        public async Task<IActionResult> CreateProductOptions([FromBody] ProductOptionTypeDTO productOptionsTypeDto)
        {
            if (productOptionsTypeDto == null || productOptionsTypeDto.Options == null)
            {
                return BadRequest();
            }

            // Check exist option type name
            if (await _productOptionService.GetExistOptionTypeName(productOptionsTypeDto.Name))
            {
                return BadRequest(new ErrorDTO() { Title = "type name already exist", Status = 400 });
            }

            try
            {
                //# Add product options type
                var newOptionsType = new ProductOptionType()
                {
                    Name = productOptionsTypeDto.Name,
                };

                var createdOptionsType = await _productOptionService.AddOptionsType(newOptionsType);

                //# Adding Product Options
                foreach (var option in productOptionsTypeDto.Options)
                {
                    var newOption = new ProductOption()
                    {
                        OptionTypeId = createdOptionsType.OptionTypeId,
                        Name = option.Name,
                        Value = option.Value
                    };

                    await _productOptionService.AddOption(newOption);
                }

                return CreatedAtAction(
                    nameof(GetProductOptions),
                    new { id = createdOptionsType.OptionTypeId },
                    new ResponseDTO()
                    {
                        Data = await ConvertToProductOptionTypeDto(createdOptionsType)
                    });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        // ----------- PUT -----------
        // PUT: api/productoptions/9
        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateProductOptions([FromRoute] int? id, [FromBody] ProductOptionTypeDTO productOptionsTypeDto)
        {
            if (
                !id.HasValue ||
                productOptionsTypeDto == null ||
                productOptionsTypeDto.Options == null
                )
            {
                return BadRequest();
            }

            //# Get exist option type
            var existOptionsType = await _productOptionService.GetOptionsTypeById(id.Value);
            if (existOptionsType == null)
            {
                return NotFound(new ErrorDTO() { Title = "options type not found", Status = 404 });
            }

            // Check exist option type name
            if (await _productOptionService.GetExistOptionTypeName(productOptionsTypeDto.Name) && existOptionsType.Name != productOptionsTypeDto.Name)
            {
                return BadRequest(new ErrorDTO() { Title = "type name already exist", Status = 400 });
            }

            try
            {
                // Get old options
                var oldOptions = await _productOptionService.GetOptions(existOptionsType);
                var oldOptionsId = oldOptions.Select(o => o.ProductOptionId).ToList();

                var newOptionsId = productOptionsTypeDto.Options.Select(o => o.ProductOptionId).ToList();

                // Remove options list: in old options and not in new options
                foreach (var option in oldOptions)
                {
                    if (!newOptionsId.Contains(option.ProductOptionId))
                    {
                        await _productOptionService.RemoveOption(option);
                    }
                }

                // Add new options list: in new options and not in old options
                foreach (var option in productOptionsTypeDto.Options)
                {
                    // Add new option
                    if (!oldOptionsId.Contains(option.ProductOptionId))
                    {
                        var newOption = new ProductOption()
                        {
                            OptionTypeId = existOptionsType.OptionTypeId,
                            Name = option.Name,
                            Value = option.Value
                        };

                        await _productOptionService.AddOption(newOption);
                    }

                    // Update old option
                    if (oldOptionsId.Contains(option.ProductOptionId))
                    {
                        var existOption = await _productOptionService.GetOption(option.ProductOptionId);
                        if (existOption == null)
                        {
                            return NotFound(new ErrorDTO() { Title = "option not found", Status = 404 });
                        }

                        var updateOption = new ProductOption()
                        {
                            ProductOptionId = existOption.ProductOptionId,
                            Name = option.Name,
                            Value = option.Value
                        };

                        if (existOption.Name != updateOption.Name || existOption.Value != updateOption.Value)
                        {
                            await _productOptionService.UpdateOption(updateOption);
                        }

                    }
                }

                // Update options type
                var newOptionsType = new ProductOptionType()
                {
                    OptionTypeId = existOptionsType.OptionTypeId,
                    Name = productOptionsTypeDto.Name,
                };

                if (existOptionsType.Name != newOptionsType.Name)
                {
                    var updatedOptionsType = await _productOptionService.UpdateOptionsType(newOptionsType);

                    return Ok(new ResponseDTO()
                    {
                        Data = await ConvertToProductOptionTypeDto(updatedOptionsType)
                    });
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToProductOptionTypeDto(existOptionsType),
                Status = 304,
                Title = "not modified",
            });
        }

        // ----------- DELETE -----------
        // DELETE: api/productoptions/9
        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemoveProductOption([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get exist options type
            var existOptionType = await _productOptionService.GetOptionsTypeById(id.Value);
            if (existOptionType == null)
            {
                return NotFound(new ErrorDTO() { Title = "options type not found", Status = 404 });
            }

            try
            {
                // Remove options type
                await _productOptionService.RemoveOptionsType(existOptionType);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToProductOptionTypeDto(existOptionType)
            });
        }
    }
}
