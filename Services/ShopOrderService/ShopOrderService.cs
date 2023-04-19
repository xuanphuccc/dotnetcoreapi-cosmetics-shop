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

        // Add
        public async Task<OrderItem> AddOrderItem(OrderItem orderItem)
        {
            await _context.OrderItems.AddAsync(orderItem);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot create order item");
            }

            return orderItem;
        }

        public async Task<ShopOrder> AddShopOrder(ShopOrder shopOrder)
        {
            await _context.ShopOrders.AddAsync(shopOrder);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception("cannot create order");
            }

            return shopOrder;
        }

        // Get
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
                .OrderByDescending(s => s.OrderDate)
                .ToListAsync();

            return userShopOrders;
        }

        // Remove
        public async Task<int> RemoveShopOrder(ShopOrder shopOrder)
        {
            _context.Remove(shopOrder);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("cannot delete order");
            }

            return result;
        }

        // Change order status
        public async Task<ShopOrder> ChangeOrderStatus(ShopOrder shopOrder, string statusCode, string statusName)
        {
            var existOrder = await GetShopOrder(shopOrder.OrderId);
            if (existOrder == null)
            {
                throw new Exception("order not found");
            }

            var orderStatus = await _context.OrderStatuses.FirstOrDefaultAsync(o => o.Status.ToLower() == statusCode.ToLower());
            if (orderStatus == null)
            {
                var newOrderStatus = new OrderStatus()
                {
                    Name = statusName,
                    Status = statusCode.ToLower(),
                };

                await _context.OrderStatuses.AddAsync(newOrderStatus);
                await _context.SaveChangesAsync();
                orderStatus = newOrderStatus;
            }

            if (orderStatus != null)
            {
                existOrder.OrderStatusId = orderStatus.OrderStatusId;
            }

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                throw new Exception($"cannot {statusCode} order");
            }

            return existOrder;
        }

        // Filter
        public IQueryable<ShopOrder> FilterAllShopOrders()
        {
            var shopOrders = _context.ShopOrders.AsQueryable();
            return shopOrders;
        }

        public IQueryable<ShopOrder> FilterSearch(IQueryable<ShopOrder> shopOrders, string search)
        {
            shopOrders = shopOrders.Where(s => s.OrderId.ToString().ToLower().Contains(search.ToLower()));

            return shopOrders;
        }

        public IQueryable<ShopOrder> FilterSortByCreationTime(IQueryable<ShopOrder> shopOrders, bool isDesc = true)
        {
            if (isDesc)
            {
                shopOrders = shopOrders.OrderByDescending(s => s.OrderDate);
            }
            else
            {
                shopOrders = shopOrders.OrderBy(s => s.OrderDate);
            }

            return shopOrders;
        }

        public IQueryable<ShopOrder> FilterSortByTotal(IQueryable<ShopOrder> shopOrders, bool isDesc = false)
        {
            if (isDesc)
            {
                shopOrders = shopOrders.OrderByDescending(s => s.OrderTotal);
            }
            else
            {
                shopOrders = shopOrders.OrderBy(s => s.OrderTotal);
            }

            return shopOrders;
        }

        public IQueryable<ShopOrder> FilterByStatus(IQueryable<ShopOrder> shopOrders, string status)
        {
            shopOrders = from s in shopOrders
                         join os in _context.OrderStatuses on s.OrderStatusId equals os.OrderStatusId
                         where os.Status == status
                         select s;

            return shopOrders;
        }
    }
}
