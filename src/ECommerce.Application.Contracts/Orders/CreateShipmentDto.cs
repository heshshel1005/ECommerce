namespace ECommerce.Orders;

public class CreateShipmentDto
{
    public string? Carrier { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Notes { get; set; }
}
