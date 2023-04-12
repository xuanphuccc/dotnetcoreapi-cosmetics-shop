using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ProductService
{
	public interface IProductService
	{
		// Get
		Task<List<Product>> GetAllProducts();
		Task<Product> GetProductById(int productId);
		Task<List<ProductCategory>> getProductCategories(Product product);
		Task<List<ProductItem>> GetAllItems(Product product);
		Task<ProductItem> GetItem(int productItemId);
		Task<List<ProductConfiguration>> GetConfigurations(ProductItem product);
        

        // Get product item promotions
        Task<List<Promotion>> GetItemPromotions(int productItemId);


        // Add
        Task<Product> AddProduct(Product product);
		Task<ProductItem> AddProductItem(ProductItem productItem);
		Task<ProductConfiguration> AddProductConfiguration(ProductConfiguration productConfiguration);
		Task<ProductCategory> AddProductCategory(ProductCategory productCategory);

		// Update
		Task<Product> UpdateProduct(Product product);
		Task<ProductItem> UpdateProductItem(ProductItem productItem);

		// Remove
		Task<int> RemoveProduct(Product product);
		Task<int> RemoveProductItem(ProductItem productItem);
		Task<bool> IsHasOrderItem(ProductItem productItem);
		Task<int> RemoveProductCategory(ProductCategory productCategory);
		Task<int> RemoveAllProductConfigurations(ProductItem productItem);

		// Convert
		Task<ProductDTO> ConvertToProductDtoAsync(Product product, int itemId = 0);


        // Filter
        IQueryable<Product> FillterAllProducts();
        
		IQueryable<Product> FilterByProviderName(IQueryable<Product> products, string providerName);
		IQueryable<Product> FilterByCategoryName(IQueryable<Product> products, string categoryName);
		IQueryable<Product> FilterByPriceRange(IQueryable<Product> products, decimal min, decimal max);

        IQueryable<Product> FilterSearch(IQueryable<Product> products, string search);
        IQueryable<Product> FilterSortByCreationTime(IQueryable<Product> products, bool isDesc = true);
        IQueryable<Product> FilterSortByName(IQueryable<Product> products, bool isDesc = false);
        IQueryable<Product> FilterByStatus(IQueryable<Product> products, string status);
    }
}
