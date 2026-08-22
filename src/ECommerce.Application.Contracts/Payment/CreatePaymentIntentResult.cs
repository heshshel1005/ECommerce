namespace ECommerce.Payment;

/// <summary>
/// Result of creating a payment intent. Frontend uses ClientSecret (Stripe) or GatewayOrderId (PayPal) to complete payment.
/// </summary>
public class CreatePaymentIntentResult
{
    public bool Success { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    /// <summary>Stripe: PaymentIntent client_secret. PayPal: not used.</summary>
    public string? ClientSecret { get; set; }
    /// <summary>Gateway order/intent id for confirm step. Stripe: pi_xxx, PayPal: order id.</summary>
    public string? GatewayPaymentId { get; set; }
    /// <summary>Publishable key or client id for frontend (e.g. Stripe publishable key, PayPal client id).</summary>
    public string? PublishableKeyOrClientId { get; set; }
}
