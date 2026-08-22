using System;

namespace ECommerce.Analytics;

public class AnalyticsFilterDto
{
    /// <summary>Filter orders created on or after this date (UTC).</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>Filter orders created before this date (UTC).</summary>
    public DateTime? DateTo { get; set; }
}

public class SalesSummaryDto
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
}

public class SalesByDayDto
{
    public DateTime Date { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}

public class TopProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
}

