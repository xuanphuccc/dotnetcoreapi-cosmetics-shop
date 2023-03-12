using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.ShopOrderService
{
	public class ShopOrderService : IShopOrderService
	{
		private readonly CosmeticsShopContext _context;
		public ShopOrderService(CosmeticsShopContext context)
		{
			_context = context;
		}

		public async Task<OrderItem> AddOrderItem(OrderItem orderItem)
		{
			await _context.OrderItems.AddAsync(orderItem);
			var result = await _context.SaveChangesAsync();
			if(result == 0)
			{
				return null!;
			}

			return orderItem;
		}

		public async Task<ShopOrder> AddShopOrder(ShopOrder shopOrder)
		{
			await _context.ShopOrders.AddAsync(shopOrder);
			var result = await _context.SaveChangesAsync();
			if (result == 0)
			{
				return null!;
			}

			return shopOrder;
		}

		public async Task<List<ShopOrder>> GetAllShopOrders()
		{
			var allShopOrders = await _context.ShopOrders.ToListAsync();
			return allShopOrders;
		}

		public async Task<OrderItem> GetOrderItem(int orderItemId)
		{
			var orderItem = await _context.OrderItems.FirstOrDefaultAsync(o => o.OrderItemId == orderItemId);
			return orderItem!;
		}

		public async Task<List<OrderItem>> GetOrderItems(ShopOrder shopOrder)
		{
			var allItems = await _context.OrderItems
				.Where(o => o.OrderId == shopOrder.OrderId)
				.ToListAsync();

			return allItems;
		}

		public async Task<ShopOrder> GetShopOrder(int shopOrderId)
		{
			var shopOrder = await _context.ShopOrders.FirstOrDefaultAsync(s => s.OrderId == shopOrderId);
			return shopOrder!;
		}

		public async Task<List<ShopOrder>> GetUserShopOrders(string userId)
		{
			var userShopOrders = await _context.ShopOrders
				.Where(s => s.UserId == userId)
				.ToListAsync();

			return userShopOrders;
		}


		// Remove
		public async Task<int> RemoveShopOrder(ShopOrder shopOrder)
		{
			_context.Remove(shopOrder);
			var result = await _context.SaveChangesAsync();
			return result;
		}
	}
}
