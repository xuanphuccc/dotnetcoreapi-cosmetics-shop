using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ProductService
{
	public interface IProductService
	{
		// Get
		Task<List<Product>> GetAllProducts();
		Task<Product> GetProductById(int productId);
		Task<List<ProductCategory>> GetAllCategories(Product product);
		Task<List<ProductItem>> GetAllItems(Product product);
		Task<ProductItem> GetItem(int productItemId);
		Task<List<ProductConfiguration>> GetConfigurations(ProductItem product);
		
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
	}
}
