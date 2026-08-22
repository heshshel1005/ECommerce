using System.Threading.Tasks;

namespace ECommerce.Payment;

/// <summary>
/// Payment gateway abstraction. No raw card data is ever passed; the client uses gateway tokens / hosted checkout.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>Gateway identifier (e.g. "Stripe", "PayPal").</summary>
    string Name { get; }

    /// <summary>Publishable key or client id for frontend (e.g. Stripe publishable key, PayPal client id). Null if not configured.</summary>
    string? PublishableKeyOrClientId { get; }

    /// <summary>
    /// Creates a payment intent/order on the gateway. Returns client secret or order id for the frontend to complete payment.
    /// </summary>
    Task<CreatePaymentIntentResult> CreatePaymentIntentAsync(CreatePaymentIntentRequest request);

    /// <summary>
    /// Confirms/captures the payment after the client has completed the flow. Updates order payment status.
    /// </summary>
    Task<ConfirmPaymentResult> ConfirmPaymentAsync(ConfirmPaymentRequest request);

    /// <summary>
    /// Refunds a previously captured payment (full or partial).
    /// </summary>
    Task<RefundPaymentResult> RefundPaymentAsync(RefundPaymentRequest request);
}
