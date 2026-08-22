using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

/// <summary>
/// Translation item for brand localized fields.
/// </summary>
public class BrandTranslationDto : INameDescriptionTranslationDto
{
    [Required]
    public string Language { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.Catalog.BrandMaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(ECommerceConsts.Catalog.BrandMaxDescriptionLength)]
    public string? Description { get; set; }
}
