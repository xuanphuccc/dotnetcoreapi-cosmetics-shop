using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.OrderStatusService;
using web_api_cosmetics_shop.Services.ShippingMethodService;

namespace web_api_cosmetics_shop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderStatusesController : ControllerBase
	{
		private readonly IOrderStatusService _orderStatusService;

		public OrderStatusesController(IOrderStatusService orderStatusService)
		{
			_orderStatusService = orderStatusService;
		}

		[NonAction]
		private OrderStatusDTO ConvertToOrderStatusDto(OrderStatus orderStatus)
		{
			var orderStatusDto = new OrderStatusDTO()
			{
				OrderStatusId = orderStatus.OrderStatusId,
				Status = orderStatus.Status
			};

			return orderStatusDto;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllOrderStatuses()
		{
			var orderStatuses = await _orderStatusService.GetAllOrderStatuses();

			List<OrderStatusDTO> orderStatusesDtos = new List<OrderStatusDTO>();
            foreach (var item in orderStatuses)
            {
				var orderStatusDto = ConvertToOrderStatusDto(item);

				orderStatusesDtos.Add(orderStatusDto);
            }

            return Ok(orderStatusesDtos);
		}

		[HttpGet("{id?}")]
		public async Task<IActionResult> GetOrderStatus([FromRoute] int? id)
		{
			if(!id.HasValue)
			{
				return BadRequest();
			}

			var orderStatus = await _orderStatusService.GetOrderStatus(id.Value);
			if(orderStatus == null)
			{
				return NotFound();
			}

			var orderStatusDto = ConvertToOrderStatusDto(orderStatus);

			return Ok(orderStatusDto);
		}

		[HttpPost]
		public async Task<IActionResult> AddOrderStatus([FromBody] OrderStatusDTO orderStatusDto)
		{
			if(orderStatusDto == null)
			{
				return BadRequest();
			}

			try
			{
				var newOrderStatus = new OrderStatus()
				{
					Status = orderStatusDto.Status
				};

				var addResult = await _orderStatusService.AddOrderStatus(newOrderStatus);
				if (addResult == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError,
												new ErrorDTO() { Title = "Can not add order status", Status = 500 });
				}

				return CreatedAtAction(
					nameof(GetOrderStatus),
					new { id = addResult.OrderStatusId },
					ConvertToOrderStatusDto(addResult));
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}

		[HttpPut("{id?}")]
		public async Task<IActionResult> UpdateOrderStatus([FromRoute] int? id, [FromBody] OrderStatusDTO orderStatusDto)
		{
			if(!id.HasValue || orderStatusDto == null)
			{
				return BadRequest();
			}

			var existOrderStatus = await _orderStatusService.GetOrderStatus(id.Value);
			if(existOrderStatus == null)
			{
				return NotFound();
			}

			// Update order status
			try
			{
				var updateOrderStatus = new OrderStatus()
				{
					OrderStatusId = existOrderStatus.OrderStatusId,
					Status = orderStatusDto.Status
				};

				if (existOrderStatus.Status != updateOrderStatus.Status)
				{
					var updateResult = await _orderStatusService.UpdateOrderStatus(updateOrderStatus);
					if (updateResult == null)
					{
						return StatusCode(StatusCodes.Status500InternalServerError,
											new ErrorDTO() { Title = "Can not update order status", Status = 500 });
					}
				}

				return Ok(orderStatusDto);
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}

		[HttpDelete("{id?}")]
		public async Task<IActionResult> RemoveOrderStatus([FromRoute] int? id)
		{
			if (!id.HasValue)
			{
				return BadRequest();
			}

			var existOrderStatus = await _orderStatusService.GetOrderStatus(id.Value);
			if (existOrderStatus == null)
			{
				return NotFound();
			}

			var hasRemove = ConvertToOrderStatusDto(existOrderStatus);

			// Delete order status
			try
			{
				var removeResult = await _orderStatusService.RemoveOrderStatus(existOrderStatus);
				if (removeResult == 0)
				{
					return StatusCode(StatusCodes.Status500InternalServerError,
										new ErrorDTO() { Title = "Can not remove order status", Status = 500 });
				}
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}

			return Ok(hasRemove);
		}
	}
}
