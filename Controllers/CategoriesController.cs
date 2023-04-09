using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.CategoryService;

namespace web_api_cosmetics_shop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoriesController(ICategoryService categoryService) {
			_categoryService = categoryService;
		}

		[HttpGet]
		public async Task<IActionResult> GetCategories()
		{
			// Get all Categories
			var categories = await _categoryService.GetCategoriesAsync();
			return Ok(categories);
		}

		[HttpGet("{id?}")]
		public async Task<IActionResult> GetCategory([FromRoute] int? id)
		{
			if(!id.HasValue) {
				return BadRequest();
			}

			// Get Category by id
			var category = await _categoryService.GetCategoryByIdAsync(id.Value);
			if(category == null)
			{
				return NotFound();
			}

			return Ok(category);
		}

		[HttpPost]
		public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO category)
		{
			if(category == null)
			{
				return BadRequest();
			}

			// Check existing Category name
			var isExistCategoryName = await _categoryService.GetExistCategoryNameAsync(category.Name);
			if(isExistCategoryName == true)
			{
				return BadRequest(new ErrorDTO() {Title = "Name already exist", Status = 400});
			}

			try
			{
				var newCategory = new Category()
				{
					Name = category.Name,
					Image = category.Image,
					PromotionId = category.PromotionId,
					CreateAt = DateTime.Now
				};

				// Creating Category
				var createdCategory = await _categoryService.AddCategory(newCategory);

				if (createdCategory == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}

				return CreatedAtAction(nameof(GetCategory), new {id = createdCategory.CategoryId}, createdCategory);
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}

		[HttpPut("{id?}")]
		public async Task<IActionResult> UpdateCategory([FromRoute] int? id, [FromBody] CategoryDTO category)
		{
			if(!id.HasValue || category == null) {
				return BadRequest();
			}

			// Find Existing Category
			var existCategory = await _categoryService.GetCategoryByIdAsync(id.Value);
			if(existCategory == null)
			{
				return NotFound();
			}

			// Check existing Category name
			var isExistCategoryName = await _categoryService.GetExistCategoryNameAsync(category.Name);
			if (isExistCategoryName == true && category.Name != existCategory.Name)
			{
				return BadRequest(new ErrorDTO() { Title = "Name already exist", Status = 400 });
			}

			try
			{
				var newCategory = new Category()
				{
					CategoryId = existCategory.CategoryId,
					Name = category.Name,
					Image = category.Image,
					PromotionId = category.PromotionId
				};

				// Updating Category
				var result = await _categoryService.UpdateCategory(newCategory);
				if (result == null)
				{
					return BadRequest(new ErrorDTO() { Title = "Can not update category", Status = 400 });
				}

				return Ok(result);
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}

		[HttpDelete("{id?}")]
		public async Task<IActionResult> RemoveCategory([FromRoute] int? id)
		{
			if (!id.HasValue)
			{
				return BadRequest();
			}

			// Find existing Category
			var existCategory = await _categoryService.GetCategoryByIdAsync(id.Value);
			if (existCategory == null)
			{
				return NotFound();
			}

			try
			{
				// Removing Category
				var result = await _categoryService.RemoveCategory(existCategory);
				if (result == null)
				{
					return BadRequest(new ErrorDTO() { Title = "Can not delete category", Status = 400 });
				}

				return Ok(result);
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}
	}
}
