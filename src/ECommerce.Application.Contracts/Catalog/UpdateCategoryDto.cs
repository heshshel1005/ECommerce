using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

/// <summary>
/// DTO for updating a category.
/// </summary>
public class UpdateCategoryDto
{
    [Required]
    [StringLength(ECommerceConsts.Catalog.CategoryMaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.Catalog.CategoryMaxSlugLength)]
    public string Slug { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }

    public int DisplayOrder { get; set; }

    public List<CategoryTranslationDto> Translations { get; set; } = new();
}
