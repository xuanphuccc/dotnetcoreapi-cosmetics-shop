using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Data;

namespace web_api_cosmetics_shop.Services.ReportService
{
    public class ReportService : IReportService
    {
        private readonly CosmeticsShopContext _context;
        public ReportService(CosmeticsShopContext context) {
            _context = context;
        }

        public async Task<int> GetTotalCustomers()
        {
            var totalCustomers = await _context.AppUsers.CountAsync();

            return totalCustomers;
        }

        public async Task<int> GetTotalOrders()
        {
            var totalOrders = await _context.ShopOrders.CountAsync();

            return totalOrders;
        }

        public async Task<decimal> GetTotalSales()
        {
            var totalSales = await _context.ShopOrders.SumAsync(s => s.OrderTotal);

            return totalSales;
        }
    }
}
