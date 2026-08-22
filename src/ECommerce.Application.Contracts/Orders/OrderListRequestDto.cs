using System;
using Volo.Abp.Application.Dtos;

namespace ECommerce.Orders;

public class OrderListRequestDto : PagedAndSortedResultRequestDto
{
    /// <summary>Filter by order status (e.g. Pending, Confirmed).</summary>
    public string? Status { get; set; }
    /// <summary>Filter orders created on or after this date (UTC).</summary>
    public DateTime? DateFrom { get; set; }
    /// <summary>Filter orders created before this date (UTC).</summary>
    public DateTime? DateTo { get; set; }
    /// <summary>Search in contact email and contact name.</summary>
    public string? Search { get; set; }
}
