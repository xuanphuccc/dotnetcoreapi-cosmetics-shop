using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ProductService
{
	public interface IProductService
	{
		// Get
		Task<List<Product>> GetAllProducts();
		Task<Product> GetProductById(int id);
		Task<List<ProductCategory>> GetCategories(Product product);
		Task<List<ProductItem>> GetItems(Product product);
		Task<List<ProductConfiguration>> GetConfigurations(ProductItem product);
		
		// Add
		Task<Product> AddProduct(Product product);
		Task<ProductItem> AddProductItem(ProductItem productItem);
		Task<ProductConfiguration> AddProductOption(ProductConfiguration productConfiguration);
		Task<ProductCategory> AddProductCategory(ProductCategory productCategory);

		// Update
		Task<Product> UpdateProduct(Product product);

		// Remove
		Task<int> RemoveProduct(Product product);
		Task<int> RemoveProductItems(Product product);
		Task<int> RemoveProductCategories(Product product);
		Task<int> RemoveProductOptions(ProductItem product);
		
	}
}
