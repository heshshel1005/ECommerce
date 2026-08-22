using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ECommerce.Orders;

/// <summary>
/// A shipment record for an order: carrier, tracking number, shipped at.
/// </summary>
public class Shipment : AuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid OrderId { get; set; }
    public string? Carrier { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedAt { get; set; }
    public string? Notes { get; set; }

    protected Shipment()
    {
    }

    public Shipment(
        Guid id,
        Guid orderId,
        string? carrier = null,
        string? trackingNumber = null,
        DateTime? shippedAt = null,
        string? notes = null)
        : base(id)
    {
        OrderId = orderId;
        Carrier = carrier;
        TrackingNumber = trackingNumber;
        ShippedAt = shippedAt ?? DateTime.UtcNow;
        Notes = notes;
    }
}
