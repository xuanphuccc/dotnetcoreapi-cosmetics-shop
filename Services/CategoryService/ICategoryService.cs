using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.CategoryService
{
	public interface ICategoryService
	{
		Task<List<Category>> GetCategoriesAsync();
		Task<Category> GetCategoryByIdAsync(int id);
		Task<bool> GetExistCategoryNameAsync(string name);
		Task<Category> AddCategory(Category category);
		Task<Category> UpdateCategory(Category category);
		Task<Category> RemoveCategory(Category category);
	}
}
