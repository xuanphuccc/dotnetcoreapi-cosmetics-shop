using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.PaymentMethodService
{
	public interface IPaymentMethodService
	{
		//Method
		Task<List<PaymentMethod>> GetAllPaymentMethods();
		Task<List<PaymentMethod>> GetUserPaymentMethods(string userId);
		Task<PaymentMethod> GetPaymentMethod(int paymentMethodId);

		// Add
		Task<PaymentMethod> AddPaymentMethod(PaymentMethod paymentMethod);

		// Update
		Task<PaymentMethod> UpdatePaymentMethod(PaymentMethod paymentMethod);

		// Delete
		Task<int> RemovePaymentMethod(PaymentMethod paymentMethod);
	}
}
