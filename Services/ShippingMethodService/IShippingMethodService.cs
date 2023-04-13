using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ShippingMethodService
{
	public interface IShippingMethodService
	{
		// Get
		Task<List<ShippingMethod>> GetAllShippingMethods();
		Task<ShippingMethod> GetShippingMethod(int shippingMethodId);

		// Add
		Task<ShippingMethod> AddShippingMethod(ShippingMethod shippingMethod);

		// Update
		Task<ShippingMethod> UpdateShippingMethod(ShippingMethod shippingMethod);

		// Delete
		Task<int> RemoveShippingMethod(ShippingMethod shippingMethod);


        // Filter
        IQueryable<ShippingMethod> FilterAllShippingMethods();
        IQueryable<ShippingMethod> FilterSearch(IQueryable<ShippingMethod> shippingMethods, string search);
        IQueryable<ShippingMethod> FilterSortByCreationTime(IQueryable<ShippingMethod> shippingMethods, bool isDesc = true);
        IQueryable<ShippingMethod> FilterSortByName(IQueryable<ShippingMethod> shippingMethods, bool isDesc = false);
        IQueryable<ShippingMethod> FilterSortByPrice(IQueryable<ShippingMethod> shippingMethods, bool isDesc = false);
    }
}
