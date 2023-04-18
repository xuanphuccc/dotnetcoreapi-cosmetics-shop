using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;
using web_api_cosmetics_shop.Models.Entities;

namespace web_api_cosmetics_shop.Services.OrderStatusService
{
	public class OrderStatusService : IOrderStatusService
	{
		private readonly CosmeticsShopContext _context;
		public OrderStatusService(CosmeticsShopContext context)
		{
			_context = context;
		}

		// Create status
		public async Task<OrderStatus> AddOrderStatus(OrderStatus orderStatus)
		{
			await _context.OrderStatuses.AddAsync(orderStatus);
			var result = await _context.SaveChangesAsync();
			if (result == 0)
			{
				throw new Exception("cannot create status");
			}

			return orderStatus;
		}

		// Get status
		public async Task<List<OrderStatus>> GetAllOrderStatuses()
		{
			var orderStatuses = await _context.OrderStatuses.ToListAsync();

			return orderStatuses;
		}

		public async Task<OrderStatus> GetOrderStatus(int orderStatusId)
		{
			var orderStatus = await _context.OrderStatuses.FirstOrDefaultAsync(o => o.OrderStatusId == orderStatusId);

			return orderStatus!;
		}

		public async Task<OrderStatus> GetOrderStatus(string status)
		{
			var orderStatus = await _context.OrderStatuses.FirstOrDefaultAsync(o => o.Status.ToLower() == status.ToLower());
			return orderStatus!;
		}

		// Delete status
        public async Task<int> RemoveOrderStatus(OrderStatus orderStatus)
		{
			_context.Remove(orderStatus);
			var result = await _context.SaveChangesAsync();

			if(result == 0)
			{
				throw new Exception("cannot delete status");
			}

			return result;
		}

		public async Task<OrderStatus> UpdateOrderStatus(OrderStatus orderStatus)
		{
			var existOrderStatus = await GetOrderStatus(orderStatus.OrderStatusId);
			if(existOrderStatus == null)
			{
                throw new Exception("status not found");
            }

			existOrderStatus.Name = orderStatus.Name;
			existOrderStatus.Status = orderStatus.Status;

			var result = await _context.SaveChangesAsync();

			if(result == 0)
			{
                throw new Exception("cannot update status");
            }

			return existOrderStatus;
		}
	}
}
