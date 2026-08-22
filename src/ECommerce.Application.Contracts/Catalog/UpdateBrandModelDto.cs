using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

/// <summary>
/// DTO for updating a brand model.
/// </summary>
public class UpdateBrandModelDto
{
    [Required]
    public Guid BrandId { get; set; }

    [Required]
    [StringLength(ECommerceConsts.Catalog.BrandModelMaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(ECommerceConsts.Catalog.BrandModelMaxCodeLength)]
    public string? Code { get; set; }

    public bool IsActive { get; set; } = true;

    public List<BrandModelTranslationDto> Translations { get; set; } = new();
}

