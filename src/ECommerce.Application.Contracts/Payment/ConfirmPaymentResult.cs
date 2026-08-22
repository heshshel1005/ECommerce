namespace ECommerce.Payment;

/// <summary>
/// Result of confirming/capturing a payment.
/// </summary>
public class ConfirmPaymentResult
{
    public bool Success { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}
