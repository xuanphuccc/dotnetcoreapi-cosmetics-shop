using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.PaymentMethodService;

namespace web_api_cosmetics_shop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentMethodsController : ControllerBase
	{
		private readonly IPaymentMethodService _paymentMethodService;
		public PaymentMethodsController(IPaymentMethodService paymentMethodService)
		{
			_paymentMethodService = paymentMethodService;
		}

		[NonAction]
		private PaymentMethodDTO ConvertToPaymentMethodDto(PaymentMethod paymentMethod)
		{
			return new PaymentMethodDTO()
			{
				PaymentMethodId = paymentMethod.PaymentMethodId,
				UserId = paymentMethod.UserId,
				PaymentTypeId = paymentMethod.PaymentTypeId,
				Provider = paymentMethod.Provider,
				AccountNumber = paymentMethod.AccountNumber,
				ExpiryDate = paymentMethod.ExpiryDate,
				IsDefault = paymentMethod.IsDefault
			};
		}

		[HttpGet]
		public async Task<IActionResult> GetAllPaymentMethods()
		{
			var paymentMethods = await _paymentMethodService.GetAllPaymentMethods();

			List<PaymentMethodDTO> paymentMethodDtos = new List<PaymentMethodDTO>();
            foreach (var item in paymentMethods)
            {
				paymentMethodDtos.Add(ConvertToPaymentMethodDto(item));
            }

            return Ok(paymentMethodDtos);
		}

		[HttpGet("{id?}")]
		public async Task<IActionResult> GetUserPaymentMethods([FromRoute] string? id)
		{
			if(string.IsNullOrEmpty(id))
			{
				return BadRequest();
			}

			var userPaymentMethods = await _paymentMethodService.GetUserPaymentMethods(id);
			List<PaymentMethodDTO> userPaymentMethodsList = new List<PaymentMethodDTO>();
            foreach (var item in userPaymentMethods)
            {
                userPaymentMethodsList.Add(ConvertToPaymentMethodDto(item));
            }

            return Ok(userPaymentMethodsList);
		}

		[HttpPost]
		public async Task<IActionResult> AddPaymentMethod([FromBody] PaymentMethodDTO paymentMethodDto)
		{
			if (paymentMethodDto == null)
			{
				return BadRequest();
			}

			try
			{
				var newPaymentMethod = new PaymentMethod()
				{
					UserId = paymentMethodDto.UserId,
					PaymentTypeId = paymentMethodDto.PaymentTypeId,
					Provider = paymentMethodDto.Provider,
					AccountNumber = paymentMethodDto.AccountNumber,
					ExpiryDate = paymentMethodDto.ExpiryDate,
					IsDefault = paymentMethodDto.IsDefault
				};

				var createdPaymentMethod = await _paymentMethodService.AddPaymentMethod(newPaymentMethod);
				if(createdPaymentMethod == null)
				{
					return StatusCode(StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not create payment method", Status = 500 });
				}

				return CreatedAtAction(nameof(GetUserPaymentMethods),
				new { id = createdPaymentMethod.PaymentMethodId },
				ConvertToPaymentMethodDto(createdPaymentMethod));
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}
		}

		[HttpPut("{id?}")]
		public async Task<IActionResult> UpdatePaymentMethod([FromRoute] int? id, [FromBody] PaymentMethodDTO paymentMethodDto)
		{
			if (!id.HasValue || paymentMethodDto == null)
			{
				return BadRequest();
			}

			var existPaymentMethod = await _paymentMethodService.GetPaymentMethod(id.Value);
			if (existPaymentMethod == null)
			{
				return NotFound();
			}

			try
			{
				var newPaymentMethod = new PaymentMethod()
				{
					PaymentMethodId = existPaymentMethod.PaymentMethodId,
					Provider = paymentMethodDto.Provider,
					AccountNumber = paymentMethodDto.AccountNumber,
					ExpiryDate = paymentMethodDto.ExpiryDate,
					IsDefault = paymentMethodDto.IsDefault
				};

				if (existPaymentMethod.Provider != newPaymentMethod.Provider ||
					existPaymentMethod.AccountNumber != newPaymentMethod.AccountNumber ||
					existPaymentMethod.ExpiryDate != newPaymentMethod.ExpiryDate ||
					existPaymentMethod.IsDefault != newPaymentMethod.IsDefault)
				{
					var updatedPaymentMethod = await _paymentMethodService.UpdatePaymentMethod(newPaymentMethod);
					if (updatedPaymentMethod == null)
					{
						return StatusCode(StatusCodes.Status500InternalServerError,
										new ErrorDTO() { Title = "Can not update payment method", Status = 500 });
					}
				}
			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}

			return Ok(paymentMethodDto);
		}

		[HttpDelete("{id?}")]
		public async Task<IActionResult> RemovePaymentMethod([FromRoute] int? id)
		{
			if (!id.HasValue)
			{
				return BadRequest();
			}

			var existPaymentMethod = await _paymentMethodService.GetPaymentMethod(id.Value);
			if (existPaymentMethod == null)
			{
				return NotFound();
			}

			try
			{
				var removedPaymentMethod = await _paymentMethodService.RemovePaymentMethod(existPaymentMethod);
				if (removedPaymentMethod == 0)
				{
					return StatusCode(StatusCodes.Status500InternalServerError,
									new ErrorDTO() { Title = "Can not remove payment method", Status = 500 });
				}

			}
			catch (Exception error)
			{
				return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
			}

			return Ok(ConvertToPaymentMethodDto(existPaymentMethod));
		}
	}
}
