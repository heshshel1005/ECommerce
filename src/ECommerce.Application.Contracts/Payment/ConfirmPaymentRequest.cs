using System;

namespace ECommerce.Payment;

/// <summary>
/// Request to confirm/capture a payment after the client has completed the gateway flow.
/// </summary>
public class ConfirmPaymentRequest
{
    public Guid OrderId { get; set; }
    /// <summary>Gateway payment id (Stripe PaymentIntent id, PayPal order id).</summary>
    public string GatewayPaymentId { get; set; } = string.Empty;
}
