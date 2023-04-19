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
        public ProductService(CosmeticsShopContext context)
        {
            _context = context;
        }

        //---------- Add ----------
        public async Task<Product> AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            var addProductResult = await _context.SaveChangesAsync();

            if (addProductResult == 0)
            {
                throw new Exception("cannot create product");
            }

            return product;
        }

        public async Task<ProductItem> AddProductItem(ProductItem productItem)
        {
            await _context.ProductItems.AddAsync(productItem);
            var addProductItemResult = await _context.SaveChangesAsync();

            if (addProductItemResult == 0)
            {
                throw new Exception("cannot create product item");
            }
            return productItem;
        }

        public async Task<ProductConfiguration> AddProductConfiguration(ProductConfiguration productConfiguration)
        {
            await _context.ProductConfigurations.AddAsync(productConfiguration);
            var addProductOptionResult = await _context.SaveChangesAsync();

            if (addProductOptionResult == 0)
            {
                throw new Exception("cannot create product configuration");
            }

            return productConfiguration;
        }

        public async Task<ProductCategory> AddProductCategory(ProductCategory productCategory)
        {
            await _context.ProductCategories.AddAsync(productCategory);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot create product category");
            }

            return productCategory;
        }
        //---------- End Add ----------


        //---------- Get ----------
        public async Task<List<Product>> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return products;
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

        // Get product item promotions
        public async Task<List<Promotion>> GetItemPromotions(int productItemId)
        {
            var promotions = await (from pi in _context.ProductItems
                                    join p in _context.Products on pi.ProductId equals p.ProductId
                                    join pc in _context.ProductCategories on p.ProductId equals pc.ProductId
                                    join c in _context.Categories on pc.CategoryId equals c.CategoryId
                                    join pr in _context.Promotions on c.PromotionId equals pr.PromotionId
                                    where pi.ProductItemId == productItemId && 
                                    pr.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= pr.EndDate
                                    select pr).ToListAsync();
            return promotions;
        }
        //---------- End Get ----------


        //---------- Remove ----------
        public async Task<int> RemoveProduct(Product product)
        {
            _context.Remove(product);
            var result = await _context.SaveChangesAsync();

            if(result == 0)
            {
                throw new Exception("cannot delete product");
            }

            return result;
        }

        public async Task<int> RemoveProductItem(ProductItem productItem)
        {
            _context.Remove(productItem);
            var result = await _context.SaveChangesAsync();

            if(result == 0)
            {
                throw new Exception("cannot delete product item");
            }

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

            if (result == 0)
            {
                throw new Exception("cannot delete product category");
            }

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
        //---------- End Remove ----------

        // ---------- Update ----------
        // Update Product
        public async Task<Product> UpdateProduct(Product product)
        {
            var existProduct = await GetProductById(product.ProductId);
            if (existProduct == null)
            {
                throw new Exception("product not found");
            }

            existProduct.Name = product.Name;
            existProduct.Description = product.Description;
            existProduct.Image = product.Image;
            existProduct.ProviderId = product.ProviderId;
            existProduct.IsDisplay = product.IsDisplay;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot update product");
            }

            return product;
        }

        // Update Product Item
        public async Task<ProductItem> UpdateProductItem(ProductItem productItem)
        {
            var existProductItem = await GetItem(productItem.ProductItemId);
            if (existProductItem == null)
            {
                throw new Exception("product item not found");
            }

            existProductItem.SKU = productItem.SKU;
            existProductItem.QtyInStock = productItem.QtyInStock;
            existProductItem.Image = productItem.Image;
            existProductItem.Price = productItem.Price;
            existProductItem.CostPrice = productItem.CostPrice;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot update product item");
            }

            return productItem;
        }
        // --------- End Update ---------


        // --------- Filter ---------
        public IQueryable<Product> FillterAllProducts()
        {
            var products = _context.Products.AsQueryable();

            return products;
        }

        public IQueryable<Product> FilterByProviderName(IQueryable<Product> products, string providerName)
        {
            products = from p in products
                       join pr in _context.Providers on p.ProviderId equals pr.ProviderId
                       where pr.Name.ToLower().Contains(providerName.ToLower())
                       select p;

            return products;
        }
        public IQueryable<Product> FilterByCategoryName(IQueryable<Product> products, string categoryName)
        {
            products = from p in products
                       join pc in _context.ProductCategories on p.ProductId equals pc.ProductId
                       join c in _context.Categories on pc.CategoryId equals c.CategoryId
                       where c.Name.ToLower().Contains(categoryName.ToLower())
                       select p;

            return products;
        }

        public IQueryable<Product> FilterByPriceRange(IQueryable<Product> products, decimal min, decimal max)
        {
            products = from p in products
                       join pi in _context.ProductItems on p.ProductId equals pi.ProductId
                       where pi.Price >= min && pi.Price <= max
                       group p by p.ProductId into g
                       select g.FirstOrDefault();

            return products;
        }

        // Search products
        public IQueryable<Product> FilterSearch(IQueryable<Product> products, string search)
        {
            products = products.Where(p => p.Name.ToLower().Contains(search.ToLower()));

            return products;
        }

        // Sort by creation time
        public IQueryable<Product> FilterSortByCreationTime(IQueryable<Product> products, bool isDesc = true)
        {
            if(isDesc)
            {
                products = products.OrderByDescending(p => p.CreateAt);
            }
            else
            {
                products = products.OrderBy(p => p.CreateAt);
            }

            return products;
        }

        // Sort by name
        public IQueryable<Product> FilterSortByName(IQueryable<Product> products, bool isDesc = false)
        {
            if(isDesc)
            {
                products = products.OrderByDescending(p => p.Name);
            }
            else
            {
                products = products.OrderBy(p => p.Name);
            }

            return products;
        }

        // Sort by status
        public IQueryable<Product> FilterByStatus(IQueryable<Product> products, string status)
        {
            if(status.ToLower() == "instock")
            {
                //products = from p in products
                //           join pi in _context.ProductItems on p.ProductId equals pi.ProductId
                //           where pi.QtyInStock > 0
                //           group p by p.ProductId into g
                //           select g.FirstOrDefault();

                //products = from p in products
                //           where p.ProductItems.Any(pi => pi.QtyInStock >= 0)
                //           group p by p.ProductId into g
                //           select g.FirstOrDefault();

                //products = products.Where(p => p != null);
            }
            else if(status.ToLower() == "soldout")
            {
                //products = from p in products
                //           join pi in _context.ProductItems on p.ProductId equals pi.ProductId
                //           where pi.QtyInStock <= 0
                //           group p by p.ProductId into g
                //           select g.FirstOrDefault();

                //products = from p in products
                //           where p.ProductItems.Any(pi => pi.QtyInStock <= 0)
                //           group p by p.ProductId into g
                //           select g.FirstOrDefault();

                //products = products.Where(p => p != null);
            }

            return products;
        }
        // --------- End Filter ---------

        // Convert DTO
        public async Task<ProductDTO> ConvertToProductDtoAsync(Product product, int itemId = 0)
        {
            // Get product categories
            var categoriesId = (await getProductCategories(product))
                                .Select(c => { return c.CategoryId != null ? c.CategoryId.Value : 0; })
                                .ToList();

            // Get product items
            var productItems = await GetAllItems(product);
            if (itemId != 0)
            {
                // Get one product item
                productItems = productItems.Where(pi => pi.ProductItemId == itemId).ToList();
            }

            // Convert productItem to productItemDTO
            List<ProductItemDTO> productItemDtos = new();
            foreach (var productItem in productItems)
            {
                // Get product options
                var productOptionsId = (await GetConfigurations(productItem))
                                        .Select(pc => { return pc.ProductOptionId != null ? pc.ProductOptionId.Value : 0; })
                                        .ToList();

                // Get product item promotion
                var promotions = await GetItemPromotions(productItem.ProductItemId);
                int maxDiscountRate = 0;
                if (promotions.Count > 0)
                {
                    maxDiscountRate = promotions.Max(p => p.DiscountRate);
                }

                var productItemDto = new ProductItemDTO()
                {
                    ProductItemId = productItem.ProductItemId,
                    ProductId = productItem.ProductId,
                    SKU = productItem.SKU,
                    QtyInStock = productItem.QtyInStock,
                    Image = productItem.Image,
                    Price = productItem.Price,
                    CostPrice = productItem.CostPrice,
                    DiscountRate = maxDiscountRate,
                    OptionsId = productOptionsId
                };

                productItemDtos.Add(productItemDto);
            }

            // Convert product to productDTO
            var productDto = new ProductDTO()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                IsDisplay = product.IsDisplay,
                ProviderId = product.ProviderId,
                CategoriesId = categoriesId,
                Items = productItemDtos,
                CreateAt = product.CreateAt
            };

            return productDto;
        }


    }
}
