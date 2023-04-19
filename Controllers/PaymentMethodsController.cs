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

            return Ok(new ResponseDTO()
            {
                Data = paymentMethodDtos
            });
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetUserPaymentMethods([FromRoute] string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var userPaymentMethods = await _paymentMethodService.GetUserPaymentMethods(id);
            List<PaymentMethodDTO> userPaymentMethodsList = new List<PaymentMethodDTO>();
            foreach (var item in userPaymentMethods)
            {
                userPaymentMethodsList.Add(_paymentMethodService.ConvertToPaymentMethodDto(item));
            }

            return Ok(new ResponseDTO()
            {
                Data = userPaymentMethodsList
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddPaymentMethod([FromBody] PaymentMethodDTO paymentMethodDto)
        {
            if (paymentMethodDto == null)
            {
                return BadRequest();
            }

            // Get user from token
            var currentIdentityUser = _userService.GetCurrentUser(HttpContext.User);
            if (currentIdentityUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
            }

            // Get user from database
            var currentUser = await _userService.GetUserByUserName(currentIdentityUser.UserName);
            if (currentUser == null)
            {
                return NotFound(new ErrorDTO() { Title = "user not found", Status = 404 });
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
                    IsDefault = paymentMethodDto.IsDefault,
                    CreateAt = DateTime.UtcNow,
                };

                var createdPaymentMethod = await _paymentMethodService.AddPaymentMethod(newPaymentMethod);

                return CreatedAtAction(nameof(GetUserPaymentMethods),
                new { id = createdPaymentMethod.PaymentMethodId },
                new ResponseDTO()
                {
                    Data = _paymentMethodService.ConvertToPaymentMethodDto(createdPaymentMethod)
                });
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
                return NotFound(new ErrorDTO() { Title = "payment not found", Status = 404 });
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

                    return Ok(new ResponseDTO()
                    {
                        Data = _paymentMethodService.ConvertToPaymentMethodDto(updatedPaymentMethod)
                    });
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _paymentMethodService.ConvertToPaymentMethodDto(existPaymentMethod),
                Status = 304,
                Title = "not modified",
            });
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
                return NotFound(new ErrorDTO() { Title = "payment not found", Status = 404 });
            }

            try
            {
                var isHasOrder = await _paymentMethodService.IsHasOrder(existPaymentMethod);

                if (isHasOrder)
                {
                    // Hide payment method
                    existPaymentMethod.IsDisplay = false;
                    await _paymentMethodService.UpdatePaymentMethod(existPaymentMethod);
                }
                else
                {
                    // Remove payment method
                    await _paymentMethodService.RemovePaymentMethod(existPaymentMethod);
                }
            }
            catch (Exception error)
            {
                return BadRequest(new ErrorDTO() { Title = error.Message, Status = 400 });
            }

            return Ok(new ResponseDTO()
            {
                Data = _paymentMethodService.ConvertToPaymentMethodDto(existPaymentMethod)
            });
        }
    }
}
