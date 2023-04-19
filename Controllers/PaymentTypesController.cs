using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.PaymentTypeService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTypesController : ControllerBase
    {
        private readonly IPaymentTypeService _paymentTypeService;

        public PaymentTypesController(IPaymentTypeService paymentTypeService)
        {
            _paymentTypeService = paymentTypeService;
        }

        [NonAction]
        public PaymentTypeDTO ConvertToPaymentTypeDto(PaymentType paymentType)
        {
            var paymentTypeDto = new PaymentTypeDTO()
            {
                PaymentTypeId = paymentType.PaymentTypeId,
                Value = paymentType.Value
            };

            return paymentTypeDto;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentTypes()
        {
            var paymentTypes = await _paymentTypeService.GetAllPaymentTypes();

            List<PaymentTypeDTO> paymentTypeDtos = new List<PaymentTypeDTO>();
            foreach (var item in paymentTypes)
            {
                paymentTypeDtos.Add(ConvertToPaymentTypeDto(item));
            }

            return Ok(new ResponseDTO()
            {
                Data = paymentTypeDtos
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetPaymentType([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var paymentType = await _paymentTypeService.GetPaymentType(id.Value);
            if (paymentType == null)
            {
                return NotFound(new ErrorDTO() { Title = "payment method not found", Status = 404 });
            }

            var paymentTypeDto = ConvertToPaymentTypeDto(paymentType);

            return Ok(new ResponseDTO()
            {
                Data = paymentTypeDto
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddPaymentType([FromBody] PaymentTypeDTO paymentTypeDto)
        {
            if (paymentTypeDto == null)
            {
                return BadRequest();
            }

            try
            {
                var newPaymentType = new PaymentType()
                {
                    Value = paymentTypeDto.Value
                };

                var createdPaymentType = await _paymentTypeService.AddPaymentType(newPaymentType);

                return CreatedAtAction(nameof(GetPaymentType),
                    new { id = createdPaymentType.PaymentTypeId },
                    new ResponseDTO()
                    {
                        Data = ConvertToPaymentTypeDto(createdPaymentType)
                    });
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

        }

        [HttpPut("{id?}")]
        public async Task<IActionResult> UpdatePaymentType([FromRoute] int? id, [FromBody] PaymentTypeDTO paymentTypeDto)
        {
            if (!id.HasValue || paymentTypeDto == null)
            {
                return BadRequest();
            }

            // Get exist payment type
            var existPaymentType = await _paymentTypeService.GetPaymentType(id.Value);
            if (existPaymentType == null)
            {
                return NotFound(new ErrorDTO() { Title = "payment method not found", Status = 404 });
            }

            try
            {
                var updatePaymentType = new PaymentType()
                {
                    PaymentTypeId = existPaymentType.PaymentTypeId,
                    Value = paymentTypeDto.Value
                };

                if (existPaymentType.Value != updatePaymentType.Value)
                {
                    var updatedPaymentTypeResult = await _paymentTypeService.UpdatePaymentType(updatePaymentType);

                    return Ok(new ResponseDTO()
                    {
                        Data = ConvertToPaymentTypeDto(updatePaymentType)
                    });
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToPaymentTypeDto(existPaymentType),
                Status = 304,
                Title = "not modified"
            });
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> RemovePaymentType([FromRoute] int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            // Get exist payment method
            var existPaymentType = await _paymentTypeService.GetPaymentType(id.Value);
            if (existPaymentType == null)
            {
                return NotFound(new ErrorDTO() { Title = "payment method not found", Status = 404 });
            }

            try
            {
                await _paymentTypeService.RemovePaymentType(existPaymentType);
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = ConvertToPaymentTypeDto(existPaymentType)
            });
        }
    }
}
