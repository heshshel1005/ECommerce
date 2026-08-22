namespace ECommerce.Payment;

/// <summary>
/// Available payment gateway for the client (e.g. to show Stripe / PayPal buttons).
/// </summary>
public class PaymentGatewayDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PublishableKeyOrClientId { get; set; }
}
