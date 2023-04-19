using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.CategoryService;
using web_api_cosmetics_shop.Services.ProductService;
using web_api_cosmetics_shop.Services.ProviderService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // ---------- Get product ----------
        [HttpGet("{id?}")]
        public async Task<IActionResult> GetProduct([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get product
            var product = await _productService.GetProductById(id.Value);
            if (product == null)
            {
                return NotFound(new ErrorDTO() { Title = "product not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await _productService.ConvertToProductDtoAsync(product),
            });
        }


        // ---------- Get All Product ----------
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] decimal? min,
            [FromQuery] decimal? max,
            [FromQuery] string? provider,
            [FromQuery] string? category,
            [FromQuery] int? page,
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? status)
        {
            int pageSize = 12;
            var productsQuery = _productService.FillterAllProducts();

            if (!page.HasValue)
            {
                page = 1;
            }

            // Filter by provider name
            if (!String.IsNullOrEmpty(provider))
            {
                productsQuery = _productService.FilterByProviderName(productsQuery, provider);
            }

            // Filter by category name
            if (!String.IsNullOrEmpty(category))
            {
                productsQuery = _productService.FilterByCategoryName(productsQuery, category);
            }

            // Filter by price range
            if (min.HasValue && max.HasValue)
            {
                productsQuery = _productService.FilterByPriceRange(productsQuery, min.Value, max.Value);
            }

            // Search products
            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = _productService.FilterSearch(productsQuery, search);
            }

            // Filter by status (in stock / sold out)
            if (!string.IsNullOrEmpty(status))
            {
                productsQuery = _productService.FilterByStatus(productsQuery, status);
            }

            // Sort products
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "creationtimedesc":
                        productsQuery = _productService.FilterSortByCreationTime(productsQuery, isDesc: true);
                        break;
                    case "creationtimeasc":
                        productsQuery = _productService.FilterSortByCreationTime(productsQuery, isDesc: false);
                        break;
                    case "namedesc":
                        productsQuery = _productService.FilterSortByName(productsQuery, isDesc: true);
                        break;
                    case "nameasc":
                        productsQuery = _productService.FilterSortByName(productsQuery, isDesc: false);
                        break;
                    default:
                        break;
                }
            }

            // Total products
            int totalProducts = await productsQuery.CountAsync();

            // Total pages = total products / page size
            int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Paging
            var pagedProducts = await productsQuery.Skip((page.Value - 1) * pageSize).Take(pageSize).ToListAsync();

            // Convert to data transfer object
            List<ProductDTO> productsDtos = new();
            foreach (var product in pagedProducts)
            {
                productsDtos.Add(await _productService.ConvertToProductDtoAsync(product));
            }

            return Ok(new ResponseDTO()
            {
                Data = productsDtos,
                TotalPages = totalPages
            });
        }

        // ---------- Create product ----------
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO productDto)
        {
            if (
                productDto == null ||
                productDto.Items == null ||
                productDto.Items.Count == 0 ||
                productDto.CategoriesId == null
                )
            {
                return BadRequest();
            }

            Product createdProduct = new();
            try
            {
                // Create product
                var product = new Product()
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Image = productDto.Image,
                    IsDisplay = productDto.IsDisplay,
                    CreateAt = DateTime.UtcNow,
                    ProviderId = productDto.ProviderId,
                };

                createdProduct = await _productService.AddProduct(product);

                // Create product categories
                foreach (var categoryId in productDto.CategoriesId)
                {
                    var productCategory = new ProductCategory()
                    {
                        ProductId = createdProduct.ProductId,
                        CategoryId = categoryId,
                    };

                    await _productService.AddProductCategory(productCategory);
                }

                // Create product items
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

                    // Create product options
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

                        await _productService.AddProductConfiguration(option);
                    }
                }

                return CreatedAtAction(
                    nameof(GetProduct),
                    new { id = createdProduct.ProductId },
                    new ResponseDTO()
                    {
                        Data = await _productService.ConvertToProductDtoAsync(createdProduct),
                        Status = 201,
                        Title = "created",
                    });
            }
            catch (Exception ex)
            {
                // Delete the product on fail
                await _productService.RemoveProduct(createdProduct);

                return BadRequest(new ErrorDTO() { Title = ex.Message, Status = 400 });
            }
        }

        // ---------- Update Product ----------
        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int? id, [FromBody] ProductDTO productDto)
        {
            if (
                !id.HasValue ||
                productDto == null ||
                productDto.Items == null ||
                productDto.Items.Count == 0 ||
                productDto.CategoriesId == null
                )
            {
                return BadRequest();
            }

            // Get exist product
            var existProduct = await _productService.GetProductById(id.Value);
            if (existProduct == null)
            {
                return NotFound(new ErrorDTO() { Title = "product not found", Status = 404 });
            }

            try
            {
                // ----- Update product categories -----
                // Get old categories
                var oldCategories = await _productService.getProductCategories(existProduct);
                var oldCategoriesId = oldCategories.Select(c => c.CategoryId).ToList();

                // Get new categories id
                var newCategoriesId = productDto.CategoriesId;

                // Remove categories list: in old categories and not in new categories
                foreach (var category in oldCategories)
                {
                    if (!newCategoriesId.Contains(category.CategoryId.Value))
                    {
                        await _productService.RemoveProductCategory(category);
                    }
                }

                // Add categories list: in new categories and not in old categories
                foreach (var categoryId in newCategoriesId)
                {
                    if (!oldCategoriesId.Contains(categoryId))
                    {
                        var productCategory = new ProductCategory()
                        {
                            ProductId = existProduct.ProductId,
                            CategoryId = categoryId,
                        };

                        await _productService.AddProductCategory(productCategory);
                    }
                }
                // ----- End update product categories -----


                // ----- Update product items and product options -----
                var oldProductItems = await _productService.GetAllItems(existProduct);
                var oldProductItemsId = oldProductItems.Select(pi => pi.ProductItemId);

                var newProductItems = productDto.Items;
                var newProductItemsId = newProductItems.Select(pi => pi.ProductItemId);

                // Remove product items list: in old product items and not in new product items
                foreach (var item in oldProductItems)
                {
                    if (!newProductItemsId.Contains(item.ProductItemId))
                    {
                        // Only delete product item if there are no orders
                        if (!(await _productService.IsHasOrderItem(item)))
                        {
                            await _productService.RemoveProductItem(item);
                        }
                        else
                        {
                            return BadRequest(new ErrorDTO() { Title = "can not delete item has order", Status = 400 });
                        }
                    }
                }

                // Add product items list: in new product items and not in old product items
                foreach (var item in newProductItems)
                {
                    // Add new item
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

                        // Create product options
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

                            await _productService.AddProductConfiguration(option);
                        }
                    }

                    // Update old item
                    if (oldProductItemsId.Contains(item.ProductItemId))
                    {
                        var existProductItem = await _productService.GetItem(item.ProductItemId);
                        if (existProductItem == null)
                        {
                            return NotFound(new ErrorDTO() { Title = "product item not found", Status = 404 });
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

                        if (
                            existProductItem.SKU != updateProductItem.SKU ||
                            existProductItem.QtyInStock != updateProductItem.QtyInStock ||
                            existProductItem.Image != updateProductItem.Image ||
                            existProductItem.Price != updateProductItem.Price ||
                            existProductItem.CostPrice != updateProductItem.CostPrice)
                        {
                            await _productService.UpdateProductItem(updateProductItem);
                        }


                        // Remove product item options
                        await _productService.RemoveAllProductConfigurations(existProductItem);

                        // Creating product item options
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

                            await _productService.AddProductConfiguration(option);
                        }

                    }
                }
                // ----- End update product items and product options -----

                // Update Product
                var newProduct = new Product()
                {
                    ProductId = existProduct.ProductId,
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Image = productDto.Image,
                    IsDisplay = productDto.IsDisplay,
                    ProviderId = productDto.ProviderId,
                };

                if (
                    existProduct.Name != newProduct.Name ||
                    existProduct.Description != newProduct.Description ||
                    existProduct.Image != newProduct.Image ||
                    existProduct.ProviderId != newProduct.ProviderId ||
                    existProduct.IsDisplay != newProduct.IsDisplay)
                {
                    var updatedProduct = await _productService.UpdateProduct(newProduct);

                    return Ok(new ResponseDTO()
                    {
                        Data = await _productService.ConvertToProductDtoAsync(updatedProduct)
                    });
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await _productService.ConvertToProductDtoAsync(existProduct),
                Status = 304,
                Title = "not modified"
            });
        }

        // ---------- Remove product ----------
        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemoveProduct([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get exist product
            var existProduct = await _productService.GetProductById(id.Value);
            if (existProduct == null)
            {
                return NotFound(new ErrorDTO() { Title = "product not found", Status = 404 });
            }

            var productItems = await _productService.GetAllItems(existProduct);

            // Check order
            foreach (var item in productItems)
            {
                if (await _productService.IsHasOrderItem(item))
                {
                    return BadRequest(new ErrorDTO() { Title = "can not delete product has order", Status = 400 });
                }
            }

            try
            {
                // Delete product
                await _productService.RemoveProduct(existProduct);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await _productService.ConvertToProductDtoAsync(existProduct)
            });
        }
    }
}
