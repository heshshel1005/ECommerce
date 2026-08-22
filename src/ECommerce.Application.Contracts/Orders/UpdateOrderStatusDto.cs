namespace ECommerce.Orders;

public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
    /// <summary>Optional tracking number when setting status to Shipped.</summary>
    public string? TrackingNumber { get; set; }
    /// <summary>Optional carrier name when setting status to Shipped.</summary>
    public string? Carrier { get; set; }
}
