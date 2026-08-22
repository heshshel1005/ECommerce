using System;
using System.Collections.Generic;

namespace ECommerce.Marketing;

public class GiftRegistryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime? EventDate { get; set; }
    public List<GiftRegistryItemDto> Items { get; set; } = new();
}
