using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.ProductService;
using web_api_cosmetics_shop.Services.ShoppingCartService;
using web_api_cosmetics_shop.Services.UserService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartsController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        public ShoppingCartsController(
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IUserService userService)
        {
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _userService = userService;
        }

        [NonAction]
        private async Task<ShoppingCartDTO> ConvertToShoppingCartDto(ShoppingCart shoppingCart)
        {
            var shoppingCartItems = await _shoppingCartService.GetAllShoppingCartItems(shoppingCart);

            List<ShoppingCartItemDTO> items = new();
            foreach (var item in shoppingCartItems)
            {
                // Get product
                ProductItem productItem = new();
                if (item.ProductItemId.HasValue)
                {
                    productItem = await _productService.GetItem(item.ProductItemId.Value);
                }

                Product product = new();
                if (productItem != null && productItem.ProductId.HasValue)
                {
                    product = await _productService.GetProductById(productItem.ProductId.Value);
                }

                ProductDTO productDto = new();
                if (productItem != null && product != null)
                {
                    productDto = await _productService.ConvertToProductDtoAsync(product, productItem.ProductItemId);
                }

                var shoppingCartItemDto = new ShoppingCartItemDTO()
                {
                    CartId = item.CartId != null ? item.CartId.Value : 0,
                    CartItemId = item.CartItemId,
                    ProductItemId = item.ProductItemId != null ? item.ProductItemId.Value : 0,
                    Qty = item.Qty,
                    Product = productDto,
                    CreateAt = item.CreateAt
                };

                items.Add(shoppingCartItemDto);
            }

            var shoppingCartDto = new ShoppingCartDTO()
            {
                CartId = shoppingCart.CartId,
                UserId = shoppingCart.UserId ?? "",
                Items = items
            };

            return shoppingCartDto;
        }

        [HttpGet]
        public async Task<IActionResult> GetShoppingCarts()
        {
            var shoppingCarts = await _shoppingCartService.GetAllShoppingCarts();

            List<ShoppingCartDTO> shoppingCartDtos = new();
            foreach (var item in shoppingCarts)
            {
                var shoppingCartDto = await ConvertToShoppingCartDto(item);

                shoppingCartDtos.Add(shoppingCartDto);
            }

            return Ok(new ResponseDTO()
            {
                Data = shoppingCartDtos
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetShoppingCart(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var shoppingCart = await _shoppingCartService.GetShoppingCart(id);
            if (shoppingCart == null)
            {
                return NotFound(new ErrorDTO() { Title = "shopping cart not found", Status = 404 });
            }

            var shoppingCartDto = await ConvertToShoppingCartDto(shoppingCart);

            return Ok(new ResponseDTO()
            {
                Data = shoppingCartDto,
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateShoppingCarts(ShoppingCartDTO shoppingCartDto)
        {
            if (
                shoppingCartDto == null ||
                shoppingCartDto.Items == null ||
                shoppingCartDto.Items.Count == 0
                )
            {
                return BadRequest();
            }

            // Logged user
            var currentIdentityUser = _userService.GetCurrentUser(HttpContext.User);
            if (currentIdentityUser == null)
            {
                return NotFound();
            }

            // Exist user from database
            var currentUser = await _userService.GetUserByUserName(currentIdentityUser.UserName);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Exist shopping cart
            var existShoppingCart = await _shoppingCartService.GetShoppingCart(currentUser.UserId);

            // If the cart does not exist, create a new cart for this user
            if (existShoppingCart == null)
            {
                try
                {
                    var newShoppingCart = new ShoppingCart()
                    {
                        UserId = currentUser.UserId,
                    };

                    var createdShoppingCart = await _shoppingCartService.AddShoppingCart(newShoppingCart);

                    // Get created shopping cart information
                    existShoppingCart = createdShoppingCart;
                }
                catch (Exception error)
                {
                    return BadRequest(new ErrorDTO() { Title = error.Message, Status = 500 });
                }
            }

            // If the productItemId does not exist, add the productItem,
            // otherwise increase the quantity of the existing product
            try
            {
                foreach (var item in shoppingCartDto.Items)
                {
                    var newShoppingCartItem = new ShoppingCartItem()
                    {
                        CartId = existShoppingCart.CartId,
                        ProductItemId = item.ProductItemId,
                        Qty = item.Qty,
                        CreateAt = DateTime.UtcNow,
                    };

                    //  Increase the quantity of the existing Product
                    var existCartItem = await _shoppingCartService.IsExistProductItem(existShoppingCart, item.ProductItemId);
                    if (existCartItem != null)
                    {
                        await _shoppingCartService.IncreaseQtyOfShoppingCartItem(existCartItem, item.Qty);
                    }
                    else
                    {
                        // Add new ProductItem
                        await _shoppingCartService.AddShoppingCartItem(newShoppingCartItem);
                    }
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 500 });
            }

            return CreatedAtAction(
                nameof(GetShoppingCart),
                new { id = shoppingCartDto.UserId },
                new ResponseDTO()
                {
                    Data = await ConvertToShoppingCartDto(existShoppingCart),
                    Status = 201,
                    Title = "created",
                });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateShoppingCart([FromBody] ShoppingCartDTO shoppingCartDto)
        {
            if (shoppingCartDto == null ||
                shoppingCartDto.Items == null)
            {
                return BadRequest();
            }

            // Logged user
            var currentIdentityUser = _userService.GetCurrentUser(HttpContext.User);
            if (currentIdentityUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            // Exist user from database
            var currentUser = await _userService.GetUserByUserName(currentIdentityUser.UserName);
            if (currentUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            // Get exist shopping cart
            var existShoppingCart = await _shoppingCartService.GetShoppingCart(currentUser.UserId);
            if (existShoppingCart == null)
            {
                return NotFound(new ErrorDTO() { Title = "shopping cart not found", Status = 404 });
            }

            var oldShoppingCartItems = await _shoppingCartService.GetAllShoppingCartItems(existShoppingCart);
            var oldShopingCartItemsId = oldShoppingCartItems.Select(sc => sc.CartItemId).ToList();

            var newShoppingCartItems = shoppingCartDto.Items;
            var newShoppingCartItemsId = newShoppingCartItems.Select(sc => sc.CartItemId).ToList();

            // Remove shopping cart items list
            try
            {
                foreach (var item in oldShoppingCartItems)
                {
                    if (!newShoppingCartItemsId.Contains(item.CartItemId))
                    {
                        await _shoppingCartService.RemoveShoppingCartItem(item);
                    }
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 500 });
            }

            // Add shopping cart items list
            try
            {
                foreach (var item in newShoppingCartItems)
                {
                    // Add new shopping cart item
                    if (!oldShopingCartItemsId.Contains(item.CartItemId))
                    {
                        var newShoppingCartItem = new ShoppingCartItem()
                        {
                            CartId = existShoppingCart.CartId,
                            ProductItemId = item.ProductItemId,
                            Qty = item.Qty,
                            CreateAt = DateTime.UtcNow,
                        };

                        // Add new ProductItem
                        await _shoppingCartService.AddShoppingCartItem(newShoppingCartItem);
                    }

                    // Update old shopping cart item
                    if (oldShopingCartItemsId.Contains(item.CartItemId))
                    {
                        var existCartItem = await _shoppingCartService.GetShoppingCartItem(item.CartItemId);
                        if (existCartItem == null)
                        {
                            return NotFound(new ErrorDTO() { Title = "shopping cart item not found", Status = 404 });
                        }

                        var updateShoppingCartItem = new ShoppingCartItem()
                        {
                            CartItemId = existCartItem.CartItemId,
                            Qty = item.Qty,
                        };

                        if (updateShoppingCartItem.Qty != existCartItem.Qty)
                        {
                            await _shoppingCartService.UpdateShoppingCartItem(updateShoppingCartItem);
                        }

                    }
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = await ConvertToShoppingCartDto(existShoppingCart)
            });
        }
    }
}


