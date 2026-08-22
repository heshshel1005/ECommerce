using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Analytics;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Analytics;

/// <summary>
/// Read-only analytics endpoints for the admin dashboard and reports.
/// </summary>
[Route("api/app/analytics")]
[Area("app")]
public class AnalyticsController : ECommerceController
{
    private readonly IAnalyticsAppService _appService;

    public AnalyticsController(IAnalyticsAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("summary")]
    public Task<SalesSummaryDto> GetSalesSummaryAsync([FromQuery] AnalyticsFilterDto input)
    {
        return _appService.GetSalesSummaryAsync(input);
    }

    [HttpGet("by-day")]
    public Task<List<SalesByDayDto>> GetSalesByDayAsync([FromQuery] AnalyticsFilterDto input)
    {
        return _appService.GetSalesByDayAsync(input);
    }

    [HttpGet("top-products")]
    public Task<List<TopProductDto>> GetTopProductsAsync([FromQuery] AnalyticsFilterDto input, [FromQuery] int maxCount = 10)
    {
        return _appService.GetTopProductsAsync(input, maxCount);
    }

    [HttpGet("export")]
    public async Task<FileContentResult> ExportSalesCsvAsync([FromQuery] AnalyticsFilterDto input)
    {
        var csv = await _appService.ExportSalesCsvAsync(input);
        var bytes = Encoding.UTF8.GetBytes(csv);
        var fileName = $"analytics-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        return File(bytes, "text/csv", fileName);
    }
}

