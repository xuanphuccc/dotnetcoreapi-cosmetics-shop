using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ShopOrderService
{
	public interface IShopOrderService
	{
		// Get
		Task<List<ShopOrder>> GetAllShopOrders();
		Task<List<ShopOrder>> GetUserShopOrders(string userId);
		Task<ShopOrder> GetShopOrder(int shopOrderId);
		Task<List<OrderItem>> GetOrderItems(ShopOrder shopOrder);
		Task<OrderItem> GetOrderItem(int orderItemId);

		// Add
		Task<ShopOrder> AddShopOrder(ShopOrder shopOrder);
		Task<OrderItem> AddOrderItem(OrderItem orderItem);

		// Remove
		Task<int> RemoveShopOrder(ShopOrder shopOrder);
	}
}
