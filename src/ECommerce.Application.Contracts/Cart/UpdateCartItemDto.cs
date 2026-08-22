using System.ComponentModel.DataAnnotations;

namespace ECommerce.Cart;

/// <summary>
/// Input for updating a cart line quantity.
/// </summary>
public class UpdateCartItemDto
{
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
