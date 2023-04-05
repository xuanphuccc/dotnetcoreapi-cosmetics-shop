using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.AddressService;
using web_api_cosmetics_shop.Services.ProductService;
using web_api_cosmetics_shop.Services.ShippingMethodService;
using web_api_cosmetics_shop.Services.ShopOrderService;
using web_api_cosmetics_shop.Services.UserService;

namespace web_api_cosmetics_shop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShopOrdersController : ControllerBase
	{
		private readonly IShopOrderService _shopOrderService;
		private readonly IProductService _productService;
		private readonly IShippingMethodService _shippingMethodService;
		private readonly IAddressService _addressService;
		private readonly IUserService _userService;
		public ShopOrdersController(
			IShopOrderService shopOrderService,
			IProductService productService,
			IShippingMethodService shippingMethodService,
			IAddressService addressService,
			IUserService userService)
		{
			_shopOrderService = shopOrderService;
			_productService = productService;
			_shippingMethodService = shippingMethodService;
			_addressService = addressService;
			_userService = userService;
		}

		[NonAction]
		private async Task<ShopOrderDTO> ConvertToShopOrderDto(ShopOrder shopOrder)
		{
			var shopOrderItems = await _shopOrderService.GetOrderItems(shopOrder);

			List<OrderItemDTO> orderItemsDto = new List<OrderItemDTO>();
            foreach (var item in shopOrderItems)
            {
				var productItem = await _productService.GetItem(item.ProductItemId.Value);
				var product = await _productService.GetProductById(productItem.ProductId.Value);
				var productDto = await _productService.ConvertToProductDtoAsync(product, productItem.ProductItemId);

				orderItemsDto.Add(new OrderItemDTO()
				{
					OrderItemId = item.OrderItemId,
					Qty = item.Qty,
					Price = item.Price,
					OrderId = item.OrderId,
					ProductItemId = item.ProductItemId,
					Product = productDto
				});
            }

			var address = await _addressService.GetAddress(shopOrder.AddressId.Value);
			var addressDto = _addressService.ConvertToAddressDto(address);

            return new ShopOrderDTO()
			{
				OrderId = shopOrder.OrderId,
				OrderDate = shopOrder.OrderDate,
				OrderTotal = shopOrder.OrderTotal,
				UserId = shopOrder.UserId,
				PaymentMethodId = shopOrder.PaymentMethodId,
				AddressId = shopOrder.AddressId,
				Address = addressDto,
				ShippingMethodId = shopOrder.ShippingMethodId,
				OrderStatusId = shopOrder.OrderStatusId,
				Items = orderItemsDto
			};
		}

		[HttpGet]
		public async Task<IActionResult> GetAllShopOrders()
		{
			var allShopOrders = await _shopOrderService.GetAllShopOrders();

			List<ShopOrderDTO> orders = new List<ShopOrderDTO>();
            foreach (var item in allShopOrders)
            {
				orders.Add(await ConvertToShopOrderDto(item));
            }

            return Ok(orders);
		}

		[HttpGet("user/{id?}")]
		public async Task<IActionResult> GetUserShopOrders([FromRoute] string? id)
		{
			if(string.IsNullOrEmpty(id))
			{
				return BadRequest();
			}

			var userShopOrders = await _shopOrderService.GetUserShopOrders(id);

			List<ShopOrderDTO> orders = new List<ShopOrderDTO>();
			foreach (var item in userShopOrders)
			{
				orders.Add(await ConvertToShopOrderDto(item));
			}

			return Ok(orders);
		}

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetOneShopOrder([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var shopOrder = await _shopOrderService.GetShopOrder(id.Value);
			if(shopOrder == null)
			{
				return NotFound();
			}

            return Ok(await ConvertToShopOrderDto(shopOrder));
        }

        [HttpPost]
		public async Task<IActionResult> AddShopOrder([FromBody] ShopOrderDTO shopOrderDto)
		{
			if(shopOrderDto == null || 
				shopOrderDto.Items == null ||
				shopOrderDto.Items.Count == 0)
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

            try
			{
				var newShopOrder = new ShopOrder()
				{
					OrderDate = DateTime.Now,
					UserId = currentUser.UserId,
					PaymentMethodId = shopOrderDto.PaymentMethodId,
					AddressId = shopOrderDto.AddressId,
					ShippingMethodId = shopOrderDto.ShippingMethodId,
					OrderStatusId = shopOrderDto.OrderStatusId
				};

				// Calculate Order Toal Price
				// Toal Items Price
				decimal totalItemsPrice = 0;
                foreach (var item in shopOrderDto.Items)
                {
					var productItem = await _productService.GetItem(item.ProductItemId.Value);
					if(productItem != null)
					{
						totalItemsPrice += productItem.Price.Value * item.Qty;
					}
				}

				// Shipping Price
				decimal shippingPrice = 0;
				var shippingMethod = await _shippingMethodService.GetShippingMethod(shopOrderDto.ShippingMethodId.Value);
				if(shippingMethod != null)
				{
					shippingPrice = shippingMethod.Price.Value;
				}

				// Total = Total Items Price + Shipping Method Price
                newShopOrder.OrderTotal = totalItemsPrice + shippingPrice;


				// Add Shop Order
				var createdShopOrder = await _shopOrderService.AddShopOrder(newShopOrder);
				if(createdShopOrder == null)
				{
					return StatusCode(
								StatusCodes.Status500InternalServerError,
								new ErrorDTO() { Title = "Can not create shop order", Status = 500 });
				}

				if(createdShopOrder != null)
				{
                    foreach (var item in shopOrderDto.Items)
                    {
						var newOrderItem = new OrderItem()
						{
							OrderId = createdShopOrder.OrderId,
							ProductItemId = item.ProductItemId,
							Qty = item.Qty
						};

						// Get order item price
						var productItem = await _productService.GetItem(item.ProductItemId.Value);
						newOrderItem.Price = productItem.Price.Value;

						var createdOrderItem = await _shopOrderService.AddOrderItem(newOrderItem);
						if(createdOrderItem == null)
						{
							return StatusCode(
								StatusCodes.Status500InternalServerError,
								new ErrorDTO() { Title = "Can not create order item", Status = 500 });
						}
                    }
                }

				return CreatedAtAction(nameof(GetUserShopOrders),
					new { id = createdShopOrder?.UserId},
					await ConvertToShopOrderDto(createdShopOrder));

			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}

		[HttpPut("cancel/{id?}")]
		public async Task<IActionResult> CancelShopOrder([FromRoute] int? id)
		{
			if(!id.HasValue)
			{
				return BadRequest();
			}

            var existShopOrder = await _shopOrderService.GetShopOrder(id.Value);
            if (existShopOrder == null)
            {
                return NotFound();
            }

			var cancelResult = await _shopOrderService.CancelShopOrder(existShopOrder);
			if(cancelResult == null)
			{
                return StatusCode(
                                StatusCodes.Status500InternalServerError,
                                new ErrorDTO() { Title = "Can not cancel order", Status = 500 });
            }

            return Ok(await ConvertToShopOrderDto(existShopOrder));
		}

		[HttpDelete("{id?}")]
		public async Task<IActionResult> RemoveShopOrder([FromRoute] int? id)
		{
			if (!id.HasValue)
			{
				return BadRequest();
			}

			var existShopOrder = await _shopOrderService.GetShopOrder(id.Value);
			if(existShopOrder == null)
			{
				return NotFound();
			}

			try
			{
				var removeShopOrderResult = await _shopOrderService.RemoveShopOrder(existShopOrder);
				if(removeShopOrderResult == 0)
				{
					return StatusCode(
								StatusCodes.Status500InternalServerError,
								new ErrorDTO() { Title = "Can not delete shop order", Status = 500 });
				}
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}

			return Ok(await ConvertToShopOrderDto(existShopOrder));
		}
	}
}
