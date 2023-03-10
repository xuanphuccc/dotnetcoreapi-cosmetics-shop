using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.ShoppingCartService;

namespace web_api_cosmetics_shop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShoppingCartsController : ControllerBase
	{
		private readonly IShoppingCartService _shoppingCartService;
		public ShoppingCartsController(IShoppingCartService shoppingCartService) {
			_shoppingCartService = shoppingCartService;
		}

		[NonAction]
		private async Task<ShoppingCartDTO> ConvertToShoppingCartDto(ShoppingCart shoppingCart)
		{
			var shoppingCartItems = await _shoppingCartService.GetShoppingCartItems(shoppingCart);

			List<ShoppingCartItemDTO> items = new List<ShoppingCartItemDTO>();
			foreach (var item in shoppingCartItems)
			{
				var shoppingCartItemDto = new ShoppingCartItemDTO()
				{
					CartId = item.CartId,
					CartItemId = item.CartItemId,
					ProductItemId = item.ProductItemId,
					Qty = item.Qty
				};

				items.Add(shoppingCartItemDto);
			}

			var shoppingCartDto = new ShoppingCartDTO()
			{
				CartId = shoppingCart.CartId,
				UserId = shoppingCart.UserId,
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
		public async Task<IActionResult> CreateShoppingCarts(ShoppingCartDTO shoppingCartDTO)
		{
			if(
				shoppingCartDTO == null || 
				shoppingCartDTO.Items == null || 
				shoppingCartDTO.UserId == null
				)
			{
				return BadRequest();
			}

			var existShoppingCart = await _shoppingCartService.GetShoppingCart(shoppingCartDTO.UserId);
			if(existShoppingCart != null)
			{
				shoppingCartDTO.CartId = existShoppingCart.CartId;
			}

			// If the cart does not exist, create a new cart for user
			if (existShoppingCart == null)
			{
				try
				{
					var newShoppingCart = new ShoppingCart()
					{
						UserId = shoppingCartDTO.UserId,
					};

					var resultCreateShoppingCart = await _shoppingCartService.AddShoppingCart(newShoppingCart);
					if (resultCreateShoppingCart == null)
					{
						return StatusCode(StatusCodes.Status500InternalServerError);
					}

					// Get created information
					existShoppingCart = resultCreateShoppingCart;
					shoppingCartDTO.CartId = resultCreateShoppingCart.CartId;
				}
				catch(Exception error)
				{
					return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
				}
			}

			// If the productItemId does not exist, add the productItem,
			// otherwise increase the quantity of the existing product
			foreach (var item in shoppingCartDTO.Items)
            {
				try
				{
					var newShoppingCartItem = new ShoppingCartItem()
					{
						CartId = shoppingCartDTO.CartId,
						ProductItemId = item.ProductItemId,
						Qty = item.Qty,
					};

					//  Increase the quantity of the existing Product
					var existCartItem = await _shoppingCartService.IsExistProductItem(item.ProductItemId.Value);
					if (existCartItem != null)
					{
						var increaseResult = await _shoppingCartService.IncreaseQtyOfShoppingCartItem(existCartItem, item.Qty);

						// Get information
						item.Qty = increaseResult;
						item.CartItemId = existCartItem.CartItemId;
						item.CartId = existCartItem.CartId;
					}
					else
					{
						// Add new ProductItem
						var addItemResult = await _shoppingCartService.AddShoppingCartItem(newShoppingCartItem);
						if (addItemResult == null)
						{
							return StatusCode(StatusCodes.Status500InternalServerError);
						}

						// Get information
						item.CartItemId = addItemResult.CartItemId;
						item.CartId = addItemResult.CartId;
					}
				}
				catch(Exception error)
				{
					return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
				}
            }

            return CreatedAtAction(nameof(GetShoppingCart), new { id = shoppingCartDTO.UserId }, shoppingCartDTO);
		}
	}
}


