using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using web_api_cosmetics_shop.Models.DTO;
using web_api_cosmetics_shop.Services.ReportService;

namespace web_api_cosmetics_shop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("total-customers")]
        public async Task<IActionResult> GetTotalCustomers()
        {
            var totalCustomers = await _reportService.GetTotalCustomers();

            return Ok(new ResponseDTO()
            {
                Data = totalCustomers
            });
        }

        [HttpGet("total-orders")]
        public async Task<IActionResult> GetTotalOrders()
        {
            var totalOrders = await _reportService.GetTotalOrders();

            return Ok(new ResponseDTO()
            {
                Data = totalOrders
            });
        }

        [HttpGet("total-sales")]
        public async Task<IActionResult> GetTotalSales()
        {
            var totalSales = await _reportService.GetTotalSales();

            return Ok(new ResponseDTO()
            {
                Data = totalSales
            });
        }
    }
}
