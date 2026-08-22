using System;

namespace ECommerce.Checkout;

/// <summary>
/// Result of submitting checkout: order id and status.
/// </summary>
public class SubmitCheckoutResultDto
{
    public Guid OrderId { get; set; }
    public string Status { get; set; } = "Pending";
}
