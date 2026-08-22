using System;

namespace ECommerce.Orders;

public class ShipmentDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string? Carrier { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreationTime { get; set; }
}
