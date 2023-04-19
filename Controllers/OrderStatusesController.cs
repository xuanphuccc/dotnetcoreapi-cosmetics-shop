using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.OrderStatusService;

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
                Name = orderStatus.Name,
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

            return Ok(new ResponseDTO()
            {
                Data = orderStatusesDtos
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetOrderStatus([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var orderStatus = await _orderStatusService.GetOrderStatus(id.Value);
            if (orderStatus == null)
            {
                return NotFound(new ErrorDTO() { Title = "status not found", Status = 404 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToOrderStatusDto(orderStatus)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddOrderStatus([FromBody] OrderStatusDTO orderStatusDto)
        {
            if (orderStatusDto == null)
            {
                return BadRequest();
            }

            try
            {
                var newOrderStatus = new OrderStatus()
                {
                    Name = orderStatusDto.Name,
                    Status = orderStatusDto.Status
                };

                var addResult = await _orderStatusService.AddOrderStatus(newOrderStatus);

                return CreatedAtAction(
                    nameof(GetOrderStatus),
                    new { id = addResult.OrderStatusId },
                    new ResponseDTO()
                    {
                        Data = ConvertToOrderStatusDto(addResult)
                    });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }
        }

        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int? id, [FromBody] OrderStatusDTO orderStatusDto)
        {
            if (!id.HasValue || orderStatusDto == null)
            {
                return BadRequest();
            }

            var existOrderStatus = await _orderStatusService.GetOrderStatus(id.Value);
            if (existOrderStatus == null)
            {
                return NotFound(new ErrorDTO() { Title = "status not found", Status = 404 });
            }

            // Update order status
            try
            {
                var updateOrderStatus = new OrderStatus()
                {
                    OrderStatusId = existOrderStatus.OrderStatusId,
                    Name = orderStatusDto.Name,
                    Status = orderStatusDto.Status
                };

                if (existOrderStatus.Status != updateOrderStatus.Status ||
                    existOrderStatus.Name != updateOrderStatus.Name)
                {
                    var updateResult = await _orderStatusService.UpdateOrderStatus(updateOrderStatus);

                    return Ok(new ResponseDTO()
                    {
                        Data = ConvertToOrderStatusDto(updateOrderStatus)
                    });
                }

            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToOrderStatusDto(existOrderStatus),
                Status = 304,
                Title = "not modified"
            });
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
                return NotFound(new ErrorDTO() { Title = "status not found", Status = 404 });
            }

            // Delete order status
            try
            {
                await _orderStatusService.RemoveOrderStatus(existOrderStatus);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToOrderStatusDto(existOrderStatus)
            });
        }
    }
}
