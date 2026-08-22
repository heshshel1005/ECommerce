namespace ECommerce.Orders;

/// <summary>
/// Order lifecycle status for fulfillment and tracking.
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
}
