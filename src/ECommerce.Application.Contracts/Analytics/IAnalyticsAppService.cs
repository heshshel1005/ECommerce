using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Analytics;

[Volo.Abp.RemoteService(IsEnabled = false)]
public interface IAnalyticsAppService : IApplicationService
{
    Task<SalesSummaryDto> GetSalesSummaryAsync(AnalyticsFilterDto input);

    Task<List<SalesByDayDto>> GetSalesByDayAsync(AnalyticsFilterDto input);

    Task<List<TopProductDto>> GetTopProductsAsync(AnalyticsFilterDto input, int maxCount = 10);

    /// <summary>
    /// Exports a CSV file with daily sales and top products for the given period.
    /// </summary>
    Task<string> ExportSalesCsvAsync(AnalyticsFilterDto input);
}

