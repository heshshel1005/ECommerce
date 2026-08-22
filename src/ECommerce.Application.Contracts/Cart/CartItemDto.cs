using System;

namespace ECommerce.Cart;

/// <summary>
/// A cart line item for API responses (includes product/variant display info).
/// </summary>
public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal? UnitPrice { get; set; }
    public int Quantity { get; set; }
    /// <summary>Available stock for this variant; null if no inventory record.</summary>
    public int? AvailableStock { get; set; }
}
