using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ProductService
{
	public class ProductService : IProductService
	{
		private readonly CosmeticsShopContext _context;
		public ProductService(CosmeticsShopContext context) {
			_context = context;
		}

		//---------- Add ----------
		public async Task<Product> AddProduct(Product product)
		{
			await _context.Products.AddAsync(product);
			var addProductResult = await _context.SaveChangesAsync();
			if(addProductResult == 0)
			{
				return null!;
			}

            return product;
		}

		public async Task<ProductItem> AddProductItem(ProductItem productItem)
		{
			await _context.ProductItems.AddAsync(productItem);
			var addProductItemResult = await _context.SaveChangesAsync();
			if (addProductItemResult == 0)
			{
				return null!;
			}
			return productItem;
		}

		public async Task<ProductConfiguration> AddProductConfiguration(ProductConfiguration productConfiguration)
		{
			await _context.ProductConfigurations.AddAsync(productConfiguration);
			var addProductOptionResult = await _context.SaveChangesAsync();
			if (addProductOptionResult == 0)
			{
				return null!;
			}

			return productConfiguration;
		}

		public async Task<ProductCategory> AddProductCategory(ProductCategory productCategory)
		{
			await _context.ProductCategories.AddAsync(productCategory);
			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}
			return productCategory;
		}


		//---------- Get ----------
		public async Task<List<Product>> GetAllProducts()
		{
			var products = await _context.Products.ToListAsync();
			return products;
		}
        public IQueryable<Product> GetAllProductsAsQueryable()

        {

            var products = _context.Products.AsQueryable();
            return products;
        }
        public IQueryable<ProductItem> GetAllProductItemsAsQueryable()
		{
			var productItems=_context.ProductItems.AsQueryable();
			return productItems;
		}
        public IQueryable<ProductCategory> GetAllProductCategoriedAsQueryable()
		{
			return _context.ProductCategories.AsQueryable();
		}


        public async Task<Product> GetProductById(int id)
		{
			var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
			return product!;
		}

		public async Task<List<ProductCategory>> getProductCategories(Product product)
		{
			var categories = await _context.ProductCategories.Where(c => c.ProductId == product.ProductId).ToListAsync();
			return categories;
		}

		public async Task<List<ProductItem>> GetAllItems(Product product)
		{
			var items = await _context.ProductItems.Where(pi => pi.ProductId == product.ProductId).ToListAsync();
			return items;
		}

		public async Task<ProductItem> GetItem(int productItemId)
		{
			var item = await _context.ProductItems
						.FirstOrDefaultAsync(pi => pi.ProductItemId == productItemId);

			return item!;
		}

		public async Task<List<ProductConfiguration>> GetConfigurations(ProductItem productItem)
		{
			var configurations = await _context.ProductConfigurations.Where(pc => pc.ProductItemId == productItem.ProductItemId).ToListAsync();
			return configurations;
		}


		//---------- Remove ----------
		public async Task<int> RemoveProduct(Product product)
		{
			_context.Remove(product);
			var result = await _context.SaveChangesAsync();

			return result;
		}

		public async Task<int> RemoveProductItem(ProductItem productItem)
		{
			_context.Remove(productItem);
			var result = await _context.SaveChangesAsync();

			return result;
		}

		public async Task<bool> IsHasOrderItem(ProductItem productItem)
		{
			var isHasOrderItem = await _context.OrderItems
								.AnyAsync(oi => oi.ProductItemId == productItem.ProductItemId);

			return isHasOrderItem;
		}

		public async Task<int> RemoveProductCategory(ProductCategory productCategory)
		{
			_context.Remove(productCategory);
			var result = await _context.SaveChangesAsync();

			return result;
		}

		public async Task<int> RemoveAllProductConfigurations(ProductItem productItem)
		{
			var productOptions = await _context.ProductConfigurations
											.Where(po => po.ProductItemId == productItem.ProductItemId)
											.ToListAsync();

			_context.RemoveRange(productOptions);
			var result = await _context.SaveChangesAsync();

			return result;
		}

		// ---------- Update ----------
		// Update Product
		public async Task<Product> UpdateProduct(Product product)
		{
			var existProduct = await GetProductById(product.ProductId);
			if(existProduct == null)
			{
				return null!;
			}

			existProduct.Name = product.Name;
			existProduct.Description = product.Description;
			existProduct.Image = product.Image;
			existProduct.ProviderId = product.ProviderId;
			existProduct.IsDisplay = product.IsDisplay;

			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return product;
		}

		// Update Product Item
		public async Task<ProductItem> UpdateProductItem(ProductItem productItem)
		{
			var existProductItem = await GetItem(productItem.ProductItemId);
			if(existProductItem == null)
			{
				return null!;
			}

			existProductItem.SKU = productItem.SKU;
			existProductItem.QtyInStock = productItem.QtyInStock;
			existProductItem.Image = productItem.Image;
			existProductItem.Price = productItem.Price;
			existProductItem.CostPrice = productItem.CostPrice;

			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return productItem;
		}


		// Convert DTO
		public async Task<ProductDTO> ConvertToProductDtoAsync(Product product, int itemId = 0)
		{
			// Getting Product Categories
			var categoriesId = (await getProductCategories(product))
								.Select(c => { return c.CategoryId != null ? c.CategoryId.Value : 0; })
								.ToList();

			// Getting Product Items
			var productItems = await GetAllItems(product);
			if(itemId != 0)
			{
				// Getting one product item
				productItems = productItems.Where(pi => pi.ProductItemId == itemId).ToList();
			}

			// Converting ProductItem to ProductItemDTO
			List<ProductItemDTO> productItemDtos = new List<ProductItemDTO>();
			foreach (var productItem in productItems)
			{
				// Getting Product Options
				var productOptionsId = (await GetConfigurations(productItem))
										.Select(pc => { return pc.ProductOptionId != null ? pc.ProductOptionId.Value : 0; })
										.ToList();

				var productItemDto = new ProductItemDTO()
				{
					ProductItemId = productItem.ProductItemId,
					ProductId = productItem.ProductId,
					SKU = productItem.SKU,
					QtyInStock = productItem.QtyInStock,
					Image = productItem.Image,
					Price = productItem.Price,
					CostPrice = productItem.CostPrice,
					OptionsId = productOptionsId
				};

				productItemDtos.Add(productItemDto);
			}

			// Converting Product to ProductDTO
			var productDto = new ProductDTO()
			{
				ProductId = product.ProductId,
				Name = product.Name,
				Description = product.Description,
				Image = product.Image,
				IsDisplay = product.IsDisplay,
				ProviderId = product.ProviderId,
				CategoriesId = categoriesId,
				Items = productItemDtos
			};

			return productDto;
		}

		
	}
}
