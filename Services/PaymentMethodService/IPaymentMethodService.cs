using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.PaymentMethodService
{
	public interface IPaymentMethodService
	{
		// Get
		Task<List<PaymentMethod>> GetAllPaymentMethods();
		Task<List<PaymentMethod>> GetUserPaymentMethods(string userId);
		Task<PaymentMethod> GetPaymentMethod(int paymentMethodId);
        Task<bool> IsHasOrder(PaymentMethod paymentMethod);

        // Add
        Task<PaymentMethod> AddPaymentMethod(PaymentMethod paymentMethod);

		// Update
		Task<PaymentMethod> UpdatePaymentMethod(PaymentMethod paymentMethod);

		// Delete
		Task<int> RemovePaymentMethod(PaymentMethod paymentMethod);
		PaymentMethodDTO ConvertToPaymentMethodDto(PaymentMethod paymentMethod);
	}
}
