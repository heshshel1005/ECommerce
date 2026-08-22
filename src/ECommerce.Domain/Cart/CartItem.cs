using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ECommerce.Cart;

/// <summary>
/// A line in a shopping cart: one product variant and quantity.
/// One line per variant per cart (quantity is updated when adding same variant again).
/// </summary>
public class CartItem : AuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }

    protected CartItem()
    {
    }

    public CartItem(Guid id, Guid cartId, Guid productVariantId, int quantity)
        : base(id)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");

        CartId = cartId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        Quantity = quantity;
    }
}
