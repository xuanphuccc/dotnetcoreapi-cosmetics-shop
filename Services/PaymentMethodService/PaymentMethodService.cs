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

		public async Task<List<PaymentMethod>> GetAllPaymentMethods()
		{
			var paymentMethods = await _context.PaymentMethods.ToListAsync();
			return paymentMethods;
		}

		public async Task<List<PaymentMethod>> GetUserPaymentMethods(string userId)
		{
			var userPaymentMethods = await _context.PaymentMethods.Where(p => p.UserId == userId).ToListAsync();
			return userPaymentMethods;
		}

		public async Task<PaymentMethod> GetPaymentMethod(int paymentMethodId)
		{
			var paymentMethod = await _context.PaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId == paymentMethodId);
			return paymentMethod!;
		}

		public async Task<int> RemovePaymentMethod(PaymentMethod paymentMethod)
		{
			_context.Remove(paymentMethod);
			var result = await _context.SaveChangesAsync();
			return result;
		}

		public async Task<PaymentMethod> UpdatePaymentMethod(PaymentMethod paymentMethod)
		{
			var existPaymentMethod = await GetPaymentMethod(paymentMethod.PaymentMethodId);
			if(existPaymentMethod == null)
			{
				return null!;
			}

			existPaymentMethod.Provider = paymentMethod.Provider;
			existPaymentMethod.AccountNumber = paymentMethod.AccountNumber;
			existPaymentMethod.ExpiryDate = paymentMethod.ExpiryDate;
			existPaymentMethod.IsDefault = paymentMethod.IsDefault;

			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return existPaymentMethod;
		}
		public PaymentMethodDTO ConvertToPaymentMethodDto(PaymentMethod paymentMethod)
		{

			return new PaymentMethodDTO()
			{
				Provider = paymentMethod.Provider,
				AccountNumber = paymentMethod.AccountNumber,
				ExpiryDate = paymentMethod.ExpiryDate,
				IsDefault = paymentMethod.IsDefault,
				PaymentTypeId = paymentMethod.PaymentTypeId,
				PaymentMethodId = paymentMethod.PaymentMethodId,

			};
		}
	}

}
