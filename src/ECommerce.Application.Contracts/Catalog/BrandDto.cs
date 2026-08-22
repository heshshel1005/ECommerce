using System;
using System.Collections.Generic;

namespace ECommerce.Catalog;

/// <summary>
/// DTO for a brand.
/// </summary>
public class BrandDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<BrandTranslationDto> Translations { get; set; } = new();
}

