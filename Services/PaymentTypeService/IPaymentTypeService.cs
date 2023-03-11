using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.PaymentTypeService
{
	public interface IPaymentTypeService
	{
		// Get
		Task<List<PaymentType>> GetAllPaymentTypes();
		Task<PaymentType> GetPaymentType(int paymentTypeId);

		// Add
		Task<PaymentType> AddPaymentType(PaymentType paymentType);

		// Update
		Task<PaymentType> UpdatePaymentType(PaymentType paymentType);

		// Delete
		Task<int> RemovePaymentType(PaymentType paymentType);
	}
}
