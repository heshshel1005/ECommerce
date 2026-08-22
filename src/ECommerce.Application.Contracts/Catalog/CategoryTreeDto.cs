using System;
using System.Collections.Generic;

namespace ECommerce.Catalog;

/// <summary>
/// DTO for a category node in the tree (with children).
/// </summary>
public class CategoryTreeDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public List<CategoryTreeDto> Children { get; set; } = new();
}
