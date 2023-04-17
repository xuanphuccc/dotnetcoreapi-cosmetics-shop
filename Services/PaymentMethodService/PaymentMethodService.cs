using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.PaymentMethodService
{
	public class PaymentMethodService : IPaymentMethodService
	{
		private readonly CosmeticsShopContext _context;
		public PaymentMethodService(CosmeticsShopContext context)
		{
			_context = context;
		}

        // Create payment method
        public async Task<PaymentMethod> AddPaymentMethod(PaymentMethod paymentMethod)
		{
			await _context.PaymentMethods.AddAsync(paymentMethod);
			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return paymentMethod;
		}

        // Get payment method
        public async Task<List<PaymentMethod>> GetAllPaymentMethods()
		{
			var paymentMethods = await _context.PaymentMethods.ToListAsync();
			return paymentMethods;
		}

		public async Task<List<PaymentMethod>> GetUserPaymentMethods(string userId)
		{
			var userPaymentMethods = await _context.PaymentMethods.Where(p => p.UserId == userId && p.IsDisplay != false).ToListAsync();
			return userPaymentMethods;
		}

		public async Task<PaymentMethod> GetPaymentMethod(int paymentMethodId)
		{
			var paymentMethod = await _context.PaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId == paymentMethodId);
			return paymentMethod!;
		}

		public async Task<bool> IsHasOrder(PaymentMethod paymentMethod)
		{
			var orderCount = await (from pm in _context.PaymentMethods
                              join ord in _context.ShopOrders on pm.PaymentMethodId equals ord.PaymentMethodId
                              where pm.PaymentMethodId == paymentMethod.PaymentMethodId
                              select pm).CountAsync();

			if(orderCount == 0)
			{
				return false;
			}

			return true;
		}


		// Remove payment method
        public async Task<int> RemovePaymentMethod(PaymentMethod paymentMethod)
		{
			_context.Remove(paymentMethod);
			var result = await _context.SaveChangesAsync();
			return result;
		}

        // Update payment method
        public async Task<PaymentMethod> UpdatePaymentMethod(PaymentMethod paymentMethod)
		{
			var existPaymentMethod = await GetPaymentMethod(paymentMethod.PaymentMethodId);
			if(existPaymentMethod == null)
			{
				return null!;
			}

			existPaymentMethod.Provider = paymentMethod.Provider;
			existPaymentMethod.CardholderName = paymentMethod.CardholderName;
			existPaymentMethod.AccountNumber = paymentMethod.AccountNumber;
			existPaymentMethod.SecurityCode = paymentMethod.SecurityCode;
			existPaymentMethod.PostalCode = paymentMethod.PostalCode;
			existPaymentMethod.ExpiryDate = paymentMethod.ExpiryDate;
			existPaymentMethod.IsDefault = paymentMethod.IsDefault;
			existPaymentMethod.IsDisplay = paymentMethod.IsDisplay;

			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return existPaymentMethod;
		}
		
		// Convert to DTO
        public PaymentMethodDTO ConvertToPaymentMethodDto(PaymentMethod paymentMethod)
        {
            return new PaymentMethodDTO()
            {
                PaymentMethodId = paymentMethod.PaymentMethodId,
                UserId = paymentMethod.UserId,
                PaymentTypeId = paymentMethod.PaymentTypeId,
                Provider = paymentMethod.Provider,
                CardholderName = paymentMethod.CardholderName,
                AccountNumber = paymentMethod.AccountNumber,
                SecurityCode = paymentMethod.SecurityCode,
                PostalCode = paymentMethod.PostalCode,
                ExpiryDate = paymentMethod.ExpiryDate,
                IsDefault = paymentMethod.IsDefault,
				CreateAt = paymentMethod.CreateAt
            };
        }
    }

}
