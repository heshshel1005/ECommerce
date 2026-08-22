using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

/// <summary>
/// DTO for updating a brand.
/// </summary>
public class UpdateBrandDto
{
    [Required]
    [StringLength(ECommerceConsts.Catalog.BrandMaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(ECommerceConsts.Catalog.BrandMaxSlugLength)]
    public string? Slug { get; set; }

    [StringLength(ECommerceConsts.Catalog.BrandMaxDescriptionLength)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public List<BrandTranslationDto> Translations { get; set; } = new();
}

