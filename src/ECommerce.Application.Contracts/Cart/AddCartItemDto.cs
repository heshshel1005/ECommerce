using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Cart;

/// <summary>
/// Input for adding or updating quantity of a variant in the cart.
/// </summary>
public class AddCartItemDto
{
    public Guid ProductVariantId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}
