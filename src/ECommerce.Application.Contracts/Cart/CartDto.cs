using System;
using System.Collections.Generic;

namespace ECommerce.Cart;

/// <summary>
/// Shopping cart with items for API responses.
/// </summary>
public class CartDto
{
    public Guid Id { get; set; }
    /// <summary>True if this cart is for an authenticated user.</summary>
    public bool IsAuthenticated { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public int ItemCount { get; set; }
}
