namespace ECommerce.Payment;

/// <summary>
/// Result of a refund request.
/// </summary>
public class RefundPaymentResult
{
    public bool Success { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}
