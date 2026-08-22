namespace ECommerce.Checkout;

/// <summary>
/// A selectable shipping method with calculated cost.
/// </summary>
public class ShippingOptionDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
