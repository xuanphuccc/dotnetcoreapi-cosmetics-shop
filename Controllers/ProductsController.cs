using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.ProductService;

namespace web_api_cosmetics_shop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;
		public ProductsController(IProductService productService) {
			_productService = productService;
		}

		// ---------- Get Product ----------
		[HttpGet("{id?}")]
		public async Task<IActionResult> GetProduct([FromRoute] int? id)
		{
			if (!id.HasValue)
			{
				return BadRequest();
			}

			// Getting Product
			var product = await _productService.GetProductById(id.Value);
			if(product == null)
			{
				return NotFound();
			}

			var productDto = await _productService.ConvertToProductDtoAsync(product);

			return Ok(productDto);
		}
		

		// ---------- Get All Product ----------
		[HttpGet]
		public async Task<IActionResult> GetProducts()
		{
			var products = await _productService.GetAllProducts();

			List<ProductDTO> productsDtos = new List<ProductDTO>();
			foreach(var product in products)
			{
				var productDto = await _productService.ConvertToProductDtoAsync(product);
				productsDtos.Add(productDto);
			}

			return Ok(productsDtos);
		}

		// ---------- Add Product ----------
		[HttpPost]
		public async Task<IActionResult> AddProduct([FromBody] ProductDTO productDto)
		{
			if(
				productDto == null || 
				productDto.Items == null || 
				productDto.Items.Count == 0 || 
				productDto.CategoriesId == null
				)
			{
				return BadRequest();
			}

			try
			{
				// Creating Product
				var product = new Product()
				{
					Name = productDto.Name,
					Description = productDto.Description,
					Image = productDto.Image,
					ProviderId = productDto.ProviderId,
					IsDisplay = productDto.IsDisplay,
				};
				var createdProduct = await _productService.AddProduct(product);
				if (createdProduct == null)
				{
					return BadRequest(new ErrorDTO() { Title = "Can not create product", Status = 500 });
				}
				// Get information
				productDto.ProductId = createdProduct.ProductId;

				// Creating Product Categories
				foreach (var categoryId in productDto.CategoriesId)
				{
					var productCategory = new ProductCategory()
					{
						ProductId = createdProduct.ProductId,
						CategoryId = categoryId,
					};

					var createdProductCategory = await _productService.AddProductCategory(productCategory);
					if (createdProductCategory == null)
					{
						return BadRequest(new ErrorDTO() { Title = "Can not create product category", Status = 500 });
					}
				}

				// Creating Product Items
				foreach (var item in productDto.Items)
				{
					var productItem = new ProductItem()
					{
						ProductId = createdProduct.ProductId,
						SKU = item.SKU,
						QtyInStock = item.QtyInStock,
						Image = item.Image,
						Price = item.Price,
						CostPrice = item.CostPrice
					};
					var createdProductItem = await _productService.AddProductItem(productItem);
					if (createdProductItem == null)
					{
						return BadRequest(new ErrorDTO() { Title = "Can not create product item", Status = 500 });
					}
					// Get information
					item.ProductItemId = createdProductItem.ProductItemId;
					item.ProductId = createdProduct.ProductId;


					// Creating Product Options
					if (item.OptionsId == null)
					{
						return BadRequest();
					}
					foreach (var optionId in item.OptionsId)
					{
						var option = new ProductConfiguration()
						{
							ProductItemId = createdProductItem.ProductItemId,
							ProductOptionId = optionId
						};

						var createdProductOption = await _productService.AddProductConfiguration(option);
						if (createdProductOption == null)
						{
							return BadRequest(new ErrorDTO() { Title = "Can not create product option", Status = 500 });
						}
					}
				}
				return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, productDto);
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}

		// ---------- Update Product ----------
		[HttpPut("{id?}")]
		public async Task<IActionResult> UpdateProduct([FromRoute] int? id, [FromBody] ProductDTO productDto)
		{
			if(
				!id.HasValue || 
				productDto == null || 
				productDto.Items == null ||
				productDto.Items.Count == 0 ||
				productDto.CategoriesId == null
				)
			{
				return BadRequest();
			}

			var existProduct = await _productService.GetProductById(id.Value);
			if(existProduct == null)
			{
				return NotFound();
			}
			// Get information
			productDto.ProductId = existProduct.ProductId;

			try
			{
				var newProduct = new Product()
				{
					ProductId = existProduct.ProductId,
					Name = productDto.Name,
					Description = productDto.Description,
					Image = productDto.Image,
					ProviderId = productDto.ProviderId,
					IsDisplay = productDto.IsDisplay
				};

				//# Update Product
				if (
					existProduct.Name != newProduct.Name ||
					existProduct.Description != newProduct.Description ||
					existProduct.Image != newProduct.Image ||
					existProduct.ProviderId != newProduct.ProviderId ||
					existProduct.IsDisplay != newProduct.IsDisplay)
				{
					var result = await _productService.UpdateProduct(newProduct);
					if (result == null)
					{
						return StatusCode(StatusCodes.Status500InternalServerError, new ErrorDTO() { Title = "Can not update product", Status = 500 });
					}
				}


				// ----- Update Product Categories -----
				// Get old Categories
				var oldCategories = await _productService.GetAllCategories(existProduct);
				var oldCategoriesId = oldCategories.Select(c => c.CategoryId).ToList();

				// Get new Categories id
				var newCategoriesId = productDto.CategoriesId;

				// Remove Categories list: in old categories and not in new categories
				foreach(var category in oldCategories)
				{
					if (!newCategoriesId.Contains(category.CategoryId!.Value))
					{
						var removeCategoryResult = await _productService.RemoveProductCategory(category);
						if(removeCategoryResult == 0)
						{
							return StatusCode(
								StatusCodes.Status500InternalServerError, 
								new ErrorDTO() { Title = "Can not update product category", Status = 500 });
						}
					}
				}

				// Add Categories list: in new categories and not in old categories
				foreach (var categoryId in newCategoriesId)
				{
					if (!oldCategoriesId.Contains(categoryId))
					{
						var productCategory = new ProductCategory()
						{
							ProductId = existProduct.ProductId,
							CategoryId = categoryId,
						};

						var createdProductCategory = await _productService.AddProductCategory(productCategory);
						if (createdProductCategory == null)
						{
							return StatusCode(
								StatusCodes.Status500InternalServerError,
								new ErrorDTO() { Title = "Can not update product category", Status = 500 });
						}
					}
				}
				// ----- End Update Product Categories -----


				// ----- Update Product Items and Product Options -----
				var oldProductItems = await _productService.GetAllItems(existProduct);
				var oldProductItemsId = oldProductItems.Select(pi => pi.ProductItemId);

				var newProductItems = productDto.Items;
				var newProductItemsId = newProductItems.Select(pi => pi.ProductItemId);

				// Remove Product Items list: in old Product Items and not in new Product Items
				foreach(var item in oldProductItems)
				{
					if(!newProductItemsId.Contains(item.ProductItemId))
					{
						// Only delete product item if there are no orders
						if (!(await _productService.IsHasOrderItem(item)))
						{
							var removeProductItemResult = await _productService.RemoveProductItem(item);
							if (removeProductItemResult == 0)
							{
								return StatusCode(
									StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not update product item", Status = 500 });
							}
						}
						else
						{
							return BadRequest(new ErrorDTO() { Title = "Can not delete item has order", Status = 400 });
						}
					}
				}

				// Add Product Items list: in new Product Items and not in old Product Items
				foreach (var item in newProductItems)
				{
					// Add new Item
					if (!oldProductItemsId.Contains(item.ProductItemId))
					{
						var productItem = new ProductItem()
						{
							ProductId = existProduct.ProductId,
							SKU = item.SKU,
							QtyInStock = item.QtyInStock,
							Image = item.Image,
							Price = item.Price,
							CostPrice = item.CostPrice
						};
						var createdProductItem = await _productService.AddProductItem(productItem);
						if (createdProductItem == null)
						{
							return StatusCode(
									StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not update product item", Status = 500 });
						}
						item.ProductItemId = createdProductItem.ProductItemId;
						item.ProductId = existProduct.ProductId;

						// Creating Product Options
						if (item.OptionsId == null)
						{
							return BadRequest(new ErrorDTO() { Title = "optionsId is required", Status = 400 });
						}
						foreach (var optionId in item.OptionsId)
						{
							var option = new ProductConfiguration()
							{
								ProductItemId = createdProductItem.ProductItemId,
								ProductOptionId = optionId
							};

							var createdProductOption = await _productService.AddProductConfiguration(option);
							if (createdProductOption == null)
							{
								return StatusCode(
									StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not create product option", Status = 500 });
							}
						}
					}

					// Update old Item
					if(oldProductItemsId.Contains(item.ProductItemId))
					{
						var existProductItem = await _productService.GetItem(item.ProductItemId);
						if (existProductItem == null)
						{
							return NotFound(new ErrorDTO() { Title = "Product item not found", Status = 404 });
						}

                        var updateProductItem = new ProductItem()
						{
							ProductItemId = existProductItem.ProductItemId,
							SKU = item.SKU,
							QtyInStock = item.QtyInStock,
							Image = item.Image,
							Price = item.Price,
							CostPrice = item.CostPrice
						};

						if(
							existProductItem.SKU != updateProductItem.SKU ||
							existProductItem.QtyInStock != updateProductItem.QtyInStock ||
							existProductItem.Image != updateProductItem.Image ||
							existProductItem.Price != updateProductItem.Price ||
							existProductItem.CostPrice != updateProductItem.CostPrice)
						{
							var updateProductItemResult = await _productService.UpdateProductItem(updateProductItem);
							if(updateProductItemResult == null)
							{
								return StatusCode(
									StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not update product item", Status = 500 });
							}
						}

                       
                        // Remove Product Item Options
                        await _productService.RemoveAllProductConfigurations(existProductItem);

						// Creating Product Item Options
						if (item.OptionsId == null)
						{
							return BadRequest(new ErrorDTO() { Title = "optionsId is required", Status = 400 });
						}
						foreach (var optionId in item.OptionsId)
						{
							var option = new ProductConfiguration()
							{
								ProductItemId = existProductItem.ProductItemId,
								ProductOptionId = optionId
							};

							var createdProductOption = await _productService.AddProductConfiguration(option);
							if (createdProductOption == null)
							{
								return StatusCode(StatusCodes.Status500InternalServerError,
												new ErrorDTO() { Title = "Can not create product option", Status = 500 });
							}
						}
						
					}
				}
				// ----- End Update Product Items and Product Options -----

				return Ok(productDto);
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}

		// ---------- Remove Product ----------
		[HttpDelete("{id?}")]
		public async Task<IActionResult> RemoveProduct([FromRoute] int? id)
		{
			if (!id.HasValue)
			{
				return BadRequest();
			}

			var product = await _productService.GetProductById(id.Value);
			if(product == null)
			{
				return NotFound();
			}

			var productItems = await _productService.GetAllItems(product);

			// Check order
            foreach (var item in productItems)
            {
				if (await _productService.IsHasOrderItem(item))
				{
					return BadRequest(new ErrorDTO() { Title = "Can not delete product has order", Status = 400 });
				}
			}

            var hasRemoveProduct = await _productService.ConvertToProductDtoAsync(product);

			try
			{
				// Removing Product
				var removeProductResult = await _productService.RemoveProduct(product);
				if (removeProductResult == 0)
				{
					return StatusCode(StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not delete product", Status = 500 });
				}

				return Ok(hasRemoveProduct);
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}
	}
}
