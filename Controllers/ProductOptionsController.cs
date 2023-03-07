using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
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

		// GET: api/productoptions
		[HttpGet]
		public async Task<IActionResult> GetProductOptions()
		{
			var allProductOptions = await _productOptionService.GetProductOptions();
			return Ok(allProductOptions);
		}

		// GET: api/productoptions/9
		[HttpGet("{id?}")]
		public async Task<IActionResult> GetProductOption([FromRoute] int? id)
		{
			if(!id.HasValue)
			{
				return BadRequest();
			}

			var productOptions = await _productOptionService.GetProductOption(id.Value);
			if(productOptions == null)
			{
				return NotFound();
			}

			return Ok(productOptions);
		}

		// POST: api/productoptions
		[HttpPost]
		public async Task<IActionResult> CreateProductOptions([FromBody] ProductOptionTypeDTO productOptions)
		{
			if(productOptions == null)
			{
				return BadRequest();
			}

			// Check exist Option Type name
			if(await _productOptionService.GetExistOptionTypeName(productOptions.Name))
			{
				return BadRequest(new ErrorDTO() { Title = "Type Name already exist", Status = 400 });
			}

			var result = await _productOptionService.AddProductOptions(productOptions);
			if(result == null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			return CreatedAtAction(nameof(GetProductOption), new { id = result.OptionTypeId }, result);
		}

		// PUT: api/productoptions/9
		[HttpPut("{id?}")]
		public async Task<IActionResult> UpdateProductOptions([FromRoute] int? id, [FromBody] ProductOptionTypeDTO productOptions)
		{
			if(!id.HasValue || productOptions == null)
			{
				return BadRequest();
			}

			// Check existing Option Type
			var existProductOptionType = await _productOptionService.GetProductOption(id.Value);
			if (existProductOptionType == null)
			{
				return NotFound();
			}

			// Check exist Option Type name
			if (await _productOptionService.GetExistOptionTypeName(productOptions.Name) && existProductOptionType.Name != productOptions.Name)
			{
				return BadRequest(new ErrorDTO() { Title = "Type Name already exist", Status = 400 });
			}

			productOptions.OptionTypeId = existProductOptionType.OptionTypeId;
			var result = await _productOptionService.UpdateProductOptions(productOptions);
			if (result == null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			return Ok(result);
		}

		// DELETE: api/productoptions/9
		[HttpDelete("{id?}")]
		public async Task<IActionResult> RemoveProductOption([FromRoute] int? id)
		{
			if(!id.HasValue)
			{
				return BadRequest();
			}

			var existProductOptionType = await _productOptionService.GetProductOption(id.Value);
			if(existProductOptionType == null)
			{
				return NotFound();
			}

			var result = await _productOptionService.RemoveProductOptions(id.Value);
			if(result == null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			return Ok(result);
		}
	}
}
