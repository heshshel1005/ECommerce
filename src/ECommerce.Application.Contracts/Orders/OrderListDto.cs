using System;

namespace ECommerce.Orders;

public class OrderListDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public decimal Total { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? UserId { get; set; }
}
