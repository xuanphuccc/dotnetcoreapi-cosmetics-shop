﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? status)
        {
            var categoriesQuery = _categoryService.FilterAllCategories();

            // Search categories
            if (!string.IsNullOrEmpty(search))
            {
                categoriesQuery = _categoryService.FilterSearch(categoriesQuery, search);
            }

            // Sort categories
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "creationtimedesc":
                        categoriesQuery = _categoryService.FilterSortByCreationTime(categoriesQuery, isDesc: true);
                        break;
                    case "creationtimeasc":
                        categoriesQuery = _categoryService.FilterSortByCreationTime(categoriesQuery, isDesc: false);
                        break;
                    case "namedesc":
                        categoriesQuery = _categoryService.FilterSortByName(categoriesQuery, isDesc: true);
                        break;
                    case "nameasc":
                        categoriesQuery = _categoryService.FilterSortByName(categoriesQuery, isDesc: false);
                        break;
                    default:
                        break;
                }
            }

            // Filter categories by sale status (on sale / no sale)
            if (!string.IsNullOrEmpty(status))
            {
                switch (status.ToLower())
                {
                    case "onsale":
                        categoriesQuery = _categoryService.FilterBySaleStatus(categoriesQuery, onSale: true);
                        break;
                    case "nosale":
                        categoriesQuery = _categoryService.FilterBySaleStatus(categoriesQuery, onSale: false);
                        break;
                    default:
                        break;
                }
            }

            // Get all Categories
            var categories = await categoriesQuery.ToListAsync();

            List<CategoryDTO> categoriesDto = new();
            foreach (var category in categories)
            {
                categoriesDto.Add(_categoryService.ConvertToCategoryDto(category));
            }

            return Ok(new ResponseDTO()
            {
                Data = categoriesDto
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetCategory([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get Category by id
            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound(new ErrorDTO() { Title = "category not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _categoryService.ConvertToCategoryDto(category)
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO category)
        {
            if (category == null)
            {
                return BadRequest();
            }

            // Check existing Category name
            var isExistCategoryName = await _categoryService.GetExistCategoryNameAsync(category.Name);
            if (isExistCategoryName == true)
            {
                return BadRequest(new ErrorDTO() { Title = "category name already exist", Status = 400 });
            }

            try
            {
                var newCategory = new Category()
                {
                    Name = category.Name,
                    Image = category.Image,
                    PromotionId = category.PromotionId,
                    CreateAt = DateTime.UtcNow,
                };

                // Creating Category
                var createdCategory = await _categoryService.AddCategory(newCategory);

                return CreatedAtAction(
                    nameof(GetCategory),
                    new { id = createdCategory.CategoryId },
                    new ResponseDTO()
                    {
                        Data = _categoryService.ConvertToCategoryDto(createdCategory)
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }
        }

        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int? id, [FromBody] CategoryDTO category)
        {
            if (!id.HasValue || category == null)
            {
                return BadRequest();
            }

            // Find Existing Category
            var existCategory = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (existCategory == null)
            {
                return NotFound(new ErrorDTO() { Title = "category not found", Status = 404 });
            }

            // Check existing Category name
            var isExistCategoryName = await _categoryService.GetExistCategoryNameAsync(category.Name);
            if (isExistCategoryName == true && category.Name != existCategory.Name)
            {
                return BadRequest(new ErrorDTO() { Title = "category name already exist", Status = 400 });
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
                if (existCategory.Name != newCategory.Name ||
                    existCategory.Image != newCategory.Image ||
                    existCategory.PromotionId != newCategory.PromotionId)
                {
                    var result = await _categoryService.UpdateCategory(newCategory);

                    return Ok(new ResponseDTO()
                    {
                        Data = _categoryService.ConvertToCategoryDto(result)
                    });
                }

            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _categoryService.ConvertToCategoryDto(existCategory),
                Status = 304,
                Title = "not modified",
            });
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemoveCategory([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Find exist Category
            var existCategory = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (existCategory == null)
            {
                return NotFound(new ErrorDTO() { Title = "category not found", Status = 404 });
            }

            try
            {
                // Remove Category
                var result = await _categoryService.RemoveCategory(existCategory);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _categoryService.ConvertToCategoryDto(existCategory)
            });
        }
    }
}
