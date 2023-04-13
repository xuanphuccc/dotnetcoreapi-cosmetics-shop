using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly CosmeticsShopContext _context;

        public CategoryService(CosmeticsShopContext context)
        {
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

            if (category == null)
            {
                return null!;
            }

            return category;
        }

        public async Task<Category> AddCategory(Category category)
        {
            await _context.AddAsync(category);

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return null!;
            }

            return category;
        }

        public async Task<Category> UpdateCategory(Category category)
        {
            var existCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);
            if (existCategory == null)
            {
                return null!;
            }

            existCategory.Name = category.Name;
            existCategory.Image = category.Image;
            existCategory.PromotionId = category.PromotionId;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return null!;
            }

            return existCategory;
        }

        public async Task<Category> RemoveCategory(Category category)
        {
            _context.Categories.Remove(category);

            int result = await _context.SaveChangesAsync();
            if (result == 0)
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

        // Filter
        public IQueryable<Category> FilterAllCategories()
        {
            var categories = _context.Categories.AsQueryable();

            return categories;
        }
        public IQueryable<Category> FilterSearch(IQueryable<Category> categories, string search)
        {
            categories = categories.Where(c => c.Name.ToLower().Contains(search.ToLower()));

            return categories;
        }
        public IQueryable<Category> FilterSortByCreationTime(IQueryable<Category> categories, bool isDesc = true)
        {
            if(isDesc)
            {
                categories = categories.OrderByDescending(c => c.CreateAt);
            }
            else
            {
                categories = categories.OrderBy(c => c.CreateAt);
            }

            return categories;
        }
        public IQueryable<Category> FilterSortByName(IQueryable<Category> categories, bool isDesc = false)
        {
            if(isDesc)
            {
                categories = categories.OrderByDescending(c => c.Name);
            }
            else
            {
                categories = categories.OrderBy(c => c.Name);
            }

            return categories;
        }
        public IQueryable<Category> FilterBySaleStatus(IQueryable<Category> categories, bool onSale = false)
        {
            if(onSale)
            {
                categories = categories.Where(c => c.PromotionId != null);
            }
            else
            {
                categories = categories.Where(c => c.PromotionId == null);
            }

            return categories;
        }
    }
}
