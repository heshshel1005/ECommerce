using System;
using System.Collections.Generic;
using ECommerce.Cart;

namespace ECommerce.Checkout;

/// <summary>
/// Checkout summary: cart snapshot, shipping options, and calculated totals.
/// </summary>
public class CheckoutSummaryDto
{
    public CartDto Cart { get; set; } = null!;
    public decimal SubTotal { get; set; }
    /// <summary>Discount from applied coupon.</summary>
    public decimal DiscountAmount { get; set; }
    /// <summary>Coupon code currently applied (if any).</summary>
    public string? AppliedCouponCode { get; set; }
    public List<ShippingOptionDto> ShippingOptions { get; set; } = new();
    public decimal TaxAmount { get; set; }
    public string? DefaultShippingMethodCode { get; set; }
}
