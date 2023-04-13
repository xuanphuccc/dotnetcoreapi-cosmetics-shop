namespace web_api_cosmetics_shop.Services.ReportService
{
    public interface IReportService
    {
        Task<int> GetTotalOrders();
        Task<int> GetTotalCustomers();
        Task<decimal> GetTotalSales();
    }
}
