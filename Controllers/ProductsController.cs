using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

			var productDto = await ConvertToProductDtoAsync(product);

			return Ok(productDto);
		}

		[NonAction]
		private async Task<ProductDTO> ConvertToProductDtoAsync(Product product)
		{
			// Getting Product Categories
			var categoriesId = (await _productService.GetCategories(product))
								.Select(c => { return c.CategoryId != null ? c.CategoryId.Value : 0; })
								.ToList();

			// Getting Product Items
			var productItems = await _productService.GetItems(product);

			// Converting ProductItem to ProductItemDTO
			List<ProductItemDTO> productItemDtos = new List<ProductItemDTO>();
			foreach (var productItem in productItems)
			{
				// Getting Product Options
				var productOptionsId = (await _productService.GetConfigurations(productItem))
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
				CategoriesId = categoriesId,
				Items = productItemDtos
			};

			return productDto;
		}

		// ---------- Get All Product ----------
		[HttpGet]
		public async Task<IActionResult> GetProducts()
		{
			var products = await _productService.GetAllProducts();

			List<ProductDTO> productsDtos = new List<ProductDTO>();
			foreach(var product in products)
			{
				var productDto = await ConvertToProductDtoAsync(product);
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

			// Creating Product
			var product = new Product()
			{
				Name = productDto.Name,
				Description = productDto.Description,
				Image = productDto.Image,
			};
			var createdProduct = await _productService.AddProduct(product);
			productDto.ProductId = createdProduct.ProductId;
			if (createdProduct == null)
			{
				return BadRequest(new ErrorDTO() { Title = "Can not create product", Status = 400 });
			}

			// Creating Product Categories
			foreach(var categoryId in productDto.CategoriesId)
			{
				var productCategory = new ProductCategory()
				{
					ProductId = createdProduct.ProductId,
					CategoryId = categoryId,
				};

				var createdProductCategory = await _productService.AddProductCategory(productCategory);
				if(createdProductCategory == null) {
					return BadRequest(new ErrorDTO() { Title = "Can not create product category", Status = 400 });
				}
			}

			// Creating Product Items
			foreach(var item in productDto.Items)
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
				item.ProductItemId = createdProductItem.ProductItemId;
				item.ProductId = createdProduct.ProductId;

				if(createdProductItem == null) { 
					return BadRequest(new ErrorDTO() { Title = "Can not create product item", Status = 400 });
				}

				// Creating Product Options
				if(item.OptionsId == null)
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

					var createdProductOption = await _productService.AddProductOption(option);
					if(createdProductOption == null)
					{
						return BadRequest(new ErrorDTO() { Title = "Can not create product option", Status = 400 });
					}
				}
			}
			return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId}, productDto);
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
			productDto.ProductId = existProduct.ProductId;
			if(existProduct == null)
			{
				return NotFound();
			}

			var newProduct = new Product()
			{
				ProductId = existProduct.ProductId,
				Name = productDto.Name,
				Description = productDto.Description,
				Image = productDto.Image
			};

			//# Update Product
			if(
				existProduct.Name != newProduct.Name ||
				existProduct.Description != newProduct.Description ||
				existProduct.Image != newProduct.Image)
			{
				var result = await _productService.UpdateProduct(newProduct);
				if(result == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
			}


			//# Update Product Categories
			// Remove old Product Categories
			await _productService.RemoveProductCategories(existProduct);

			// Creating new Product Categories
			foreach (var categoryId in productDto.CategoriesId)
			{
				var productCategory = new ProductCategory()
				{
					ProductId = existProduct.ProductId,
					CategoryId = categoryId,
				};

				var createdProductCategory = await _productService.AddProductCategory(productCategory);
				if (createdProductCategory == null)
				{
					return BadRequest(new ErrorDTO() { Title = "Can not create product category", Status = 500 });
				}
			}

			//# Update Product Items and Product Options
			// Remove old Product Items and Product Options
			var existItems = await _productService.GetItems(existProduct);
			foreach(var item in existItems)
			{
				await _productService.RemoveProductOptions(item);
			}

			var removeOldItemsResult = await _productService.RemoveProductItems(existProduct);
			if(removeOldItemsResult == 0)
			{
				return BadRequest(new ErrorDTO() { Title = "Can not remove product items", Status = 500 });
			}

			// Creating Product Items
			foreach (var item in productDto.Items)
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
				item.ProductItemId = createdProductItem.ProductItemId;
				item.ProductId = existProduct.ProductId;

				if (createdProductItem == null)
				{
					return BadRequest(new ErrorDTO() { Title = "Can not create product item", Status = 400 });
				}

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

					var createdProductOption = await _productService.AddProductOption(option);
					if (createdProductOption == null)
					{
						return BadRequest(new ErrorDTO() { Title = "Can not create product option", Status = 400 });
					}
				}
			}

			return Ok(productDto);
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

			var productItems = await _productService.GetItems(product);

			var hasRemoveProduct = await ConvertToProductDtoAsync(product);

			// Removing Categories
            await _productService.RemoveProductCategories(product);

			// Removing Product Options
            foreach (var item in productItems)
            {
				await _productService.RemoveProductOptions(item);
            }

			// Removing Product Items
			var removeItemsResult = await _productService.RemoveProductItems(product);
			if (removeItemsResult == 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			// Removing Product
			var removeProductResult = await _productService.RemoveProduct(product);
			if (removeProductResult == 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			return Ok(hasRemoveProduct);
		}
	}
}
