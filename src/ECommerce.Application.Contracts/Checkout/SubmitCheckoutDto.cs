using System.ComponentModel.DataAnnotations;

namespace ECommerce.Checkout;

/// <summary>
/// Submit checkout: contact, addresses, shipping method. Payment will be integrated in a later step.
/// </summary>
public class SubmitCheckoutDto
{
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string ContactEmail { get; set; } = string.Empty;

    [StringLength(32)]
    public string? ContactPhone { get; set; }

    [StringLength(256)]
    public string? ContactName { get; set; }

    [Required]
    public CheckoutAddressDto ShippingAddress { get; set; } = null!;

    /// <summary>If true, billing is same as shipping; otherwise use BillingAddress.</summary>
    public bool BillingSameAsShipping { get; set; } = true;

    public CheckoutAddressDto? BillingAddress { get; set; }

    [Required]
    [StringLength(64)]
    public string ShippingMethodCode { get; set; } = string.Empty;

    /// <summary>Optional coupon code to apply at checkout.</summary>
    [StringLength(64)]
    public string? CouponCode { get; set; }
}
