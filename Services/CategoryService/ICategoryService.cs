using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.CategoryService
{
	public interface ICategoryService
	{
		// Get
		Task<List<Category>> GetCategoriesAsync();
		Task<Category> GetCategoryByIdAsync(int id);
		Task<bool> GetExistCategoryNameAsync(string name);

		// Add new
		Task<Category> AddCategory(Category category);

		// Update
		Task<Category> UpdateCategory(Category category);

		// Remove
		Task<Category> RemoveCategory(Category category);

		// Convert to DTO
		CategoryDTO ConvertToCategoryDto(Category category);

        // Filter
        IQueryable<Category> FilterAllCategories();
        IQueryable<Category> FilterSearch(IQueryable<Category> categories, string search);
        IQueryable<Category> FilterSortByCreationTime(IQueryable<Category> categories, bool isDesc = true);
        IQueryable<Category> FilterSortByName(IQueryable<Category> categories, bool isDesc = false);
        IQueryable<Category> FilterBySaleStatus(IQueryable<Category> categories, bool onSale = false);
    }
}
