using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Orders;
using ECommerce.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Analytics;

/// <summary>
/// Read-only analytics over orders for the admin dashboard and reports.
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize(ECommercePermissions.Analytics.Default)]
public class AnalyticsAppService : ApplicationService, IAnalyticsAppService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<OrderLine, Guid> _orderLineRepository;

    public AnalyticsAppService(
        IRepository<Order, Guid> orderRepository,
        IRepository<OrderLine, Guid> orderLineRepository)
    {
        _orderRepository = orderRepository;
        _orderLineRepository = orderLineRepository;
    }

    public async Task<SalesSummaryDto> GetSalesSummaryAsync(AnalyticsFilterDto input)
    {
        var query = await BuildFilteredOrderQueryAsync(input);

        var totalOrders = await AsyncExecuter.CountAsync(query);
        var totalRevenue = totalOrders == 0
            ? 0m
            : await AsyncExecuter.SumAsync(query.Select(o => o.Total));

        return new SalesSummaryDto
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            PeriodStart = input.DateFrom,
            PeriodEnd = input.DateTo
        };
    }

    public async Task<List<SalesByDayDto>> GetSalesByDayAsync(AnalyticsFilterDto input)
    {
        var query = await BuildFilteredOrderQueryAsync(input);

        var grouped = query
            .GroupBy(o => o.CreationTime.Date)
            .Select(g => new SalesByDayDto
            {
                Date = g.Key,
                OrderCount = g.Count(),
                Revenue = g.Sum(o => o.Total)
            })
            .OrderBy(x => x.Date);

        return await AsyncExecuter.ToListAsync(grouped);
    }

    public async Task<List<TopProductDto>> GetTopProductsAsync(AnalyticsFilterDto input, int maxCount = 10)
    {
        var orderQuery = await BuildFilteredOrderQueryAsync(input);
        var orderIdsQuery = orderQuery.Select(o => o.Id);

        var linesQuery = await _orderLineRepository.GetQueryableAsync();
        linesQuery = linesQuery.Where(l => orderIdsQuery.Contains(l.OrderId));

        var grouped = linesQuery
            .GroupBy(l => new { l.ProductId, l.ProductName })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                Quantity = g.Sum(l => l.Quantity),
                Revenue = g.Sum(l => l.LineTotal)
            })
            .OrderByDescending(x => x.Quantity)
            .ThenByDescending(x => x.Revenue)
            .Take(maxCount <= 0 ? 10 : maxCount);

        return await AsyncExecuter.ToListAsync(grouped);
    }

    public async Task<string> ExportSalesCsvAsync(AnalyticsFilterDto input)
    {
        var summary = await GetSalesSummaryAsync(input);
        var byDay = await GetSalesByDayAsync(input);
        var topProducts = await GetTopProductsAsync(input, 20);

        var builder = new System.Text.StringBuilder();

        builder.AppendLine("Section,Metric,Value");
        builder.AppendLine($"Summary,TotalOrders,{summary.TotalOrders}");
        builder.AppendLine($"Summary,TotalRevenue,{summary.TotalRevenue}");

        builder.AppendLine();
        builder.AppendLine("SalesByDay,Date,OrderCount,Revenue");
        foreach (var day in byDay)
        {
            builder.AppendLine($"SalesByDay,{day.Date:yyyy-MM-dd},{day.OrderCount},{day.Revenue}");
        }

        builder.AppendLine();
        builder.AppendLine("TopProducts,ProductId,ProductName,Quantity,Revenue");
        foreach (var product in topProducts)
        {
            var name = product.ProductName?.Replace("\"", "\"\"") ?? string.Empty;
            builder.AppendLine(
                $"TopProducts,{product.ProductId},\"{name}\",{product.Quantity},{product.Revenue}");
        }

        return builder.ToString();
    }

    private async Task<IQueryable<Order>> BuildFilteredOrderQueryAsync(AnalyticsFilterDto input)
    {
        var query = await _orderRepository.GetQueryableAsync();

        if (input.DateFrom.HasValue)
        {
            query = query.Where(o => o.CreationTime >= input.DateFrom.Value);
        }

        if (input.DateTo.HasValue)
        {
            query = query.Where(o => o.CreationTime < input.DateTo.Value);
        }

        return query;
    }
}

