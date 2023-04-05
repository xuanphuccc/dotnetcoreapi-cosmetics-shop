using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;
using web_api_cosmetics_shop.Services.PaymentMethodService;
using web_api_cosmetics_shop.Services.UserService;

namespace web_api_cosmetics_shop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentMethodsController : ControllerBase
	{
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IUserService _userService;
		public PaymentMethodsController(IPaymentMethodService paymentMethodService, IUserService userService)
		{
			_paymentMethodService = paymentMethodService;
			_userService = userService;
		}
		

		[HttpGet]
		public async Task<IActionResult> GetAllPaymentMethods()
		{
			var paymentMethods = await _paymentMethodService.GetAllPaymentMethods();

			List<PaymentMethodDTO> paymentMethodDtos = new List<PaymentMethodDTO>();
            foreach (var item in paymentMethods)
            {
				paymentMethodDtos.Add(_paymentMethodService.ConvertToPaymentMethodDto(item));
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
                userPaymentMethodsList.Add(_paymentMethodService.ConvertToPaymentMethodDto(item));
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

            var currentIdentityUser = _userService.GetCurrentUser(HttpContext.User);
            if (currentIdentityUser == null)
            {
                return NotFound();
            }

            var currentUser = await _userService.GetUserByUserName(currentIdentityUser.UserName);
            if (currentUser == null)
            {
                return NotFound();
            }

            try
			{
				var newPaymentMethod = new PaymentMethod()
				{
					UserId = currentUser.UserId,
					PaymentTypeId = paymentMethodDto.PaymentTypeId,
					Provider = paymentMethodDto.Provider,
					CardholderName = paymentMethodDto.CardholderName,
					AccountNumber = paymentMethodDto.AccountNumber,
					SecurityCode = paymentMethodDto.SecurityCode,
					PostalCode = paymentMethodDto.PostalCode,
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
				_paymentMethodService.ConvertToPaymentMethodDto(createdPaymentMethod));
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
					PaymentTypeId = paymentMethodDto.PaymentTypeId,
					Provider = paymentMethodDto.Provider,
					CardholderName = paymentMethodDto.CardholderName,
					AccountNumber = paymentMethodDto.AccountNumber,
					SecurityCode = paymentMethodDto.SecurityCode,
					PostalCode = paymentMethodDto.PostalCode,
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

			return Ok(_paymentMethodService.ConvertToPaymentMethodDto(existPaymentMethod));
		}
	}
}
