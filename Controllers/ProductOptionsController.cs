using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
		public ProductOptionsController(IProductOptionService productOptionService) {
			_productOptionService = productOptionService;
		}

		[NonAction]
		private async Task<ProductOptionTypeDTO> ConvertToProductOptionTypeDtoAsync(ProductOptionType optionsType)
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
		public async Task<IActionResult> GetAllProductOptions()
		{
			var allOptionsType = await _productOptionService.GetAllOptionsTypes();

			List<ProductOptionTypeDTO> productOptionTypeDtos = new List<ProductOptionTypeDTO>();
			foreach(var type in allOptionsType)
			{
				var productOptionTypeDto = await ConvertToProductOptionTypeDtoAsync(type);
				productOptionTypeDtos.Add(productOptionTypeDto);
			}

			return Ok(productOptionTypeDtos);
		}

		// ----------- GET -----------
		// GET: api/productoptions/9
		[HttpGet("{id?}")]
		public async Task<IActionResult> GetProductOptions([FromRoute] int? id)
		{
			if(!id.HasValue)
			{
				return BadRequest();
			}

			// Get Options Type
			var optionsType = await _productOptionService.GetOptionsTypeById(id.Value);
			if(optionsType == null)
			{
				return NotFound();
			}

			var productOptionTypeDto = await ConvertToProductOptionTypeDtoAsync(optionsType);

			return Ok(productOptionTypeDto);
		}

		// ----------- POST -----------
		// POST: api/productoptions
		[HttpPost]
		public async Task<IActionResult> CreateProductOptions([FromBody] ProductOptionTypeDTO productOptionsTypeDto)
		{
			if(productOptionsTypeDto == null || productOptionsTypeDto.Options == null)
			{
				return BadRequest();
			}

			// Check exist Option Type name
			if(await _productOptionService.GetExistOptionTypeName(productOptionsTypeDto.Name))
			{
				return BadRequest(new ErrorDTO() { Title = "Type Name already exist", Status = 400 });
			}

			// Adding Product Options Type
			var newOptionsType = new ProductOptionType()
			{
				Name = productOptionsTypeDto.Name,
			};

			var createdOptionsType = await _productOptionService.AddOptionsType(newOptionsType);
			productOptionsTypeDto.OptionTypeId = createdOptionsType.OptionTypeId;
			if (createdOptionsType == null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			// Adding Product Options
            foreach (var option in productOptionsTypeDto.Options)
            {

				var newOption = new ProductOption()
				{
					OptionTypeId = createdOptionsType.OptionTypeId,
					Name = option.Name,
					Value = option.Value
				};

				var createdOption = await _productOptionService.AddOption(newOption);
				option.ProductOptionId = createdOption.ProductOptionId;
				option.OptionTypeId = createdOption.OptionTypeId;
				if (createdOption == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}

			return CreatedAtAction(nameof(GetProductOptions), new { id = createdOptionsType.OptionTypeId }, productOptionsTypeDto);
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

			// Check existing Option Type
			var existOptionsType = await _productOptionService.GetOptionsTypeById(id.Value);
			productOptionsTypeDto.OptionTypeId = existOptionsType.OptionTypeId;
			if (existOptionsType == null)
			{
				return NotFound();
			}

			// Check exist Option Type name
			if (await _productOptionService.GetExistOptionTypeName(productOptionsTypeDto.Name) && existOptionsType.Name != productOptionsTypeDto.Name)
			{
				return BadRequest(new ErrorDTO() { Title = "Type Name already exist", Status = 400 });
			}

			// Update Options Type
			var newOptionsType = new ProductOptionType()
			{
				OptionTypeId = existOptionsType.OptionTypeId,
				Name = productOptionsTypeDto.Name,
			};

			if(existOptionsType.Name != newOptionsType.Name)
			{
				var updateOptionsTypeResult = await _productOptionService.UpdateOptionsType(newOptionsType);
				if(updateOptionsTypeResult == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}

			// Remove old Options
			await _productOptionService.RemoveOptions(existOptionsType);

			// Add new Options
			foreach (var option in productOptionsTypeDto.Options)
			{

				var newOption = new ProductOption()
				{
					OptionTypeId = existOptionsType.OptionTypeId,
					Name = option.Name,
					Value = option.Value
				};

				var createdOption = await _productOptionService.AddOption(newOption);
				option.ProductOptionId = createdOption.ProductOptionId;
				option.OptionTypeId = createdOption.OptionTypeId;
				if (createdOption == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}

			return Ok(productOptionsTypeDto);
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

			var existOptionType = await _productOptionService.GetOptionsTypeById(id.Value);
			if (existOptionType == null)
			{
				return NotFound();
			}

			var hasRemove = await ConvertToProductOptionTypeDtoAsync(existOptionType);

			// Removing Options
			await _productOptionService.RemoveOptions(existOptionType);

			// Removing Options Type
			var result = await _productOptionService.RemoveOptionsType(existOptionType);
			if (result == 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			return Ok(hasRemove);
		}
	}
}
