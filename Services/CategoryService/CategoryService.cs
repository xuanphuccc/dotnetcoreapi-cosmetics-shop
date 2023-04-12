using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.CategoryService
{
	public class CategoryService : ICategoryService
	{
		private readonly CosmeticsShopContext _context;

		public CategoryService(CosmeticsShopContext context) {
			_context = context;
		}

		public async Task<List<Category>> GetCategoriesAsync()
		{
			var categories = await _context.Categories.ToListAsync();
			return categories;
		}

		public async Task<Category> GetCategoryByIdAsync(int id)
		{
			var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);

			if(category == null)
			{
				return null!;
			}

			return category;
		}
		
        public async Task<Category> AddCategory(Category category)
		{
			await _context.AddAsync(category);

			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return category;
		}

		public async Task<Category> UpdateCategory(Category category)
		{
			var existCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);
			if(existCategory == null)
			{
				return null!;
			}

			existCategory.Name = category.Name;
			existCategory.Image = category.Image;
			existCategory.PromotionId = category.PromotionId;

			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return existCategory;
		}

		public async Task<Category> RemoveCategory(Category category)
		{
			_context.Categories.Remove(category);

			int result =  await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return category;
		}

		public async Task<bool> GetExistCategoryNameAsync(string name)
		{
			var isExistName = await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());
			return isExistName;

		}
	}
}
