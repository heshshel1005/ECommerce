using System;

namespace ECommerce.Payment;

/// <summary>
/// Request to refund a captured payment.
/// </summary>
public class RefundPaymentRequest
{
    public Guid OrderId { get; set; }
    /// <summary>Gateway payment id (e.g. Stripe PaymentIntent id) for the refund call.</summary>
    public string? GatewayPaymentId { get; set; }
    /// <summary>Optional partial amount. If null, full refund.</summary>
    public decimal? Amount { get; set; }
    public string? Reason { get; set; }
}
