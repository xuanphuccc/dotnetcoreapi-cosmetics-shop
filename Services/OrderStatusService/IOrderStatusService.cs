using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.OrderStatusService
{
	public interface IOrderStatusService
	{
		// Get
		Task<List<OrderStatus>> GetAllOrderStatuses();
		Task<OrderStatus> GetOrderStatus(int orderStatusId);
		Task<OrderStatus> GetOrderStatus(string status);

		// Add
		Task<OrderStatus> AddOrderStatus(OrderStatus orderStatus);

		// Update
		Task<OrderStatus> UpdateOrderStatus(OrderStatus orderStatus);

		// Delete
		Task<int> RemoveOrderStatus(OrderStatus orderStatus);
	}
}
