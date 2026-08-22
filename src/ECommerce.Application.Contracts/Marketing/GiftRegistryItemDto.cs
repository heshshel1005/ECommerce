using System;

namespace ECommerce.Marketing;

public class GiftRegistryItemDto
{
    public Guid Id { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public int DesiredQuantity { get; set; }
    public int QuantityClaimed { get; set; }
    public int QuantityRemaining { get; set; }
    public string? Note { get; set; }
}
