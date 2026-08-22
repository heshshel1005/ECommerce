using System;

namespace ECommerce.Orders;

public class OrderStatusHistoryDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }
}
