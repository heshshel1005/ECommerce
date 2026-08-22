using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Marketing;

public class ClaimRegistryItemDto
{
    public Guid GiftRegistryItemId { get; set; }
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    [StringLength(256)]
    public string? ClaimantName { get; set; }
    [StringLength(500)]
    public string? Message { get; set; }
    /// <summary>If true, add the item to the current user's cart (and optionally proceed to checkout).</summary>
    public bool AddToCart { get; set; }
}
