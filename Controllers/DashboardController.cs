using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryAPI.Services;

namespace PharmacyInventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _dashboardService.GetSummary();
            return Ok(summary);
        }

        [HttpGet("stock-report")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStockReport()
        {
            var report = await _dashboardService.GetStockReport();
            return Ok(report);
        }
    }
}