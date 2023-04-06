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
			IUserService userService) {
			_shoppingCartService = shoppingCartService;
			_productService = productService;
			_userService = userService;
		}

		[NonAction]
		private async Task<ShoppingCartDTO> ConvertToShoppingCartDto(ShoppingCart shoppingCart)
		{
			var shoppingCartItems = await _shoppingCartService.GetAllShoppingCartItems(shoppingCart);

			List<ShoppingCartItemDTO> items = new List<ShoppingCartItemDTO>();
			foreach (var item in shoppingCartItems)
			{
				// Get product
				var productItem = await _productService.GetItem(item.ProductItemId!.Value);
				var product = await _productService.GetProductById(productItem.ProductId.Value);
				var productDto = await _productService.ConvertToProductDtoAsync(product, item.ProductItemId.Value);

				var shoppingCartItemDto = new ShoppingCartItemDTO()
				{
					CartId = item.CartId!.Value,
					CartItemId = item.CartItemId,
					ProductItemId = item.ProductItemId.Value,
					Qty = item.Qty,
					Product = productDto
				};

				items.Add(shoppingCartItemDto);
			}

			var shoppingCartDto = new ShoppingCartDTO()
			{
				CartId = shoppingCart.CartId,
				UserId = shoppingCart.UserId!,
				Items = items
			};

			return shoppingCartDto;
		}

		[HttpGet]	
		public async Task<IActionResult> GetShoppingCarts()
		{
			var shoppingCarts = await _shoppingCartService.GetAllShoppingCarts();

			List<ShoppingCartDTO> shoppingCartDtos = new List<ShoppingCartDTO>();
            foreach (var item in shoppingCarts)
            {
				var shoppingCartDto = await ConvertToShoppingCartDto(item);

				shoppingCartDtos.Add(shoppingCartDto);
            }

            return Ok(shoppingCartDtos);
		}

		[HttpGet("{id?}")]
		public async Task<IActionResult> GetShoppingCart(string? id)
		{
			if(string.IsNullOrEmpty(id))
			{
				return BadRequest();
			}

			var shoppingCart = await _shoppingCartService.GetShoppingCart(id);
			if(shoppingCart == null)
			{
				return NotFound();
			}

			var shoppingCartDto = await ConvertToShoppingCartDto(shoppingCart);

            return Ok(shoppingCartDto);
		}

		[HttpPost]
		public async Task<IActionResult> CreateShoppingCarts(ShoppingCartDTO shoppingCartDto)
		{
			if(
				shoppingCartDto == null || 
				shoppingCartDto.Items == null || 
				shoppingCartDto.Items.Count == 0 ||
				shoppingCartDto.UserId == null
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
			if(existShoppingCart != null)
			{
				shoppingCartDto.CartId = existShoppingCart.CartId;
			}

            // If the cart does not exist, create a new cart for user
            if (existShoppingCart == null)
			{
				try
				{
					var newShoppingCart = new ShoppingCart()
					{
						UserId = currentUser.UserId,
					};

					var resultCreateShoppingCart = await _shoppingCartService.AddShoppingCart(newShoppingCart);
					if (resultCreateShoppingCart == null)
					{
						return StatusCode(
								StatusCodes.Status500InternalServerError,
								new ErrorDTO() { Title = "Can not create shopping cart", Status = 500 });
					}

					// Get created information
					existShoppingCart = resultCreateShoppingCart;
					shoppingCartDto.CartId = resultCreateShoppingCart.CartId;
				}
				catch(Exception error)
				{
					return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
				}
			}

			// If the productItemId does not exist, add the productItem,
			// otherwise increase the quantity of the existing product
			foreach (var item in shoppingCartDto.Items)
            {
				try
				{
					var newShoppingCartItem = new ShoppingCartItem()
					{
						CartId = existShoppingCart.CartId,
						ProductItemId = item.ProductItemId,
						Qty = item.Qty,
					};

					//  Increase the quantity of the existing Product
					var existCartItem = await _shoppingCartService.IsExistProductItem(item.ProductItemId);
					if (existCartItem != null)
					{
						var increaseResult = await _shoppingCartService.IncreaseQtyOfShoppingCartItem(existCartItem, item.Qty);

						// Get information
						item.Qty = increaseResult;
						item.CartItemId = existCartItem.CartItemId;
						item.CartId = existCartItem.CartId!.Value;
					}
					else
					{
						// Add new ProductItem
						var addItemResult = await _shoppingCartService.AddShoppingCartItem(newShoppingCartItem);
						if (addItemResult == null)
						{
							return StatusCode(
									StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not add shopping cart item", Status = 500 });
						}

						// Get information
						item.CartItemId = addItemResult.CartItemId;
						item.CartId = addItemResult.CartId!.Value;
					}
				}
				catch(Exception error)
				{
					return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
				}
            }

            return CreatedAtAction(nameof(GetShoppingCart), new { id = shoppingCartDto.UserId }, shoppingCartDto);
		}

		[HttpPut]
		public async Task<IActionResult> UpdateShoppingCart( [FromBody] ShoppingCartDTO shoppingCartDto)
		{
			if(
				
				shoppingCartDto == null ||
				shoppingCartDto.Items == null ||
				shoppingCartDto.UserId == null)
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

			var existShoppingCart = await _shoppingCartService.GetShoppingCart(currentUser.UserId);
			if(existShoppingCart == null)
			{
				return NotFound();
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
						var removeOldCartItemResult = await _shoppingCartService.RemoveShoppingCartItem(item);
						if (removeOldCartItemResult == 0)
						{
							return StatusCode(
									StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not delete shopping cart item", Status = 500 });
						}
					}
				}
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
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
						};

						// Add new ProductItem
						var addItemResult = await _shoppingCartService.AddShoppingCartItem(newShoppingCartItem);
						if (addItemResult == null)
						{
							return StatusCode(
									StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not delete shopping cart item", Status = 500 });
						}

						// Get information
						item.CartItemId = addItemResult.CartItemId;
						item.CartId = addItemResult.CartId!.Value;
					}

					// Update old shopping cart item
					if (oldShopingCartItemsId.Contains(item.CartItemId))
					{
						var existCartItem = await _shoppingCartService.GetShoppingCartItem(item.CartItemId);
						if(existCartItem == null)
						{
							return NotFound(new ErrorDTO() { Title = "Shopping cart item not found", Status = 404 });
						}

						var updateShoppingCartItem = new ShoppingCartItem()
						{
							CartItemId = existCartItem.CartItemId,
							Qty = item.Qty,
						};

						if(updateShoppingCartItem.Qty != existCartItem.Qty)
						{
							var updateItemResult = await _shoppingCartService.UpdateShoppingCartItem(updateShoppingCartItem);
							if (updateItemResult == null)
							{
								return StatusCode(
										StatusCodes.Status500InternalServerError,
										new ErrorDTO() { Title = "Can not update shopping cart item", Status = 500 });
							}
						}

					}
				}
			}
			catch(Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
			
			return Ok(shoppingCartDto);
		}
	}
}


