using System;

namespace ECommerce.Payment;

/// <summary>
/// Request to create a payment intent (Stripe) or order (PayPal) for an order.
/// </summary>
public class CreatePaymentIntentRequest
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
    public string? CustomerEmail { get; set; }
    public string? Description { get; set; }
}
