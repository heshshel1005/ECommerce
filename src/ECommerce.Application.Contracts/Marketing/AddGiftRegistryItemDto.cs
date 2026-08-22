using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Marketing;

public class AddGiftRegistryItemDto
{
    public Guid ProductVariantId { get; set; }
    [Range(1, int.MaxValue)]
    public int DesiredQuantity { get; set; } = 1;
    [StringLength(500)]
    public string? Note { get; set; }
}
