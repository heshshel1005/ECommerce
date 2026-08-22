using System;
using System.Collections.Generic;

namespace ECommerce.Marketing;

public class WishlistDto
{
    public Guid Id { get; set; }
    public List<WishlistItemDto> Items { get; set; } = new();
}
