using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

/// <summary>
/// Translation item for brand model localized fields.
/// </summary>
public class BrandModelTranslationDto : INameTranslationDto
{
    [Required]
    public string Language { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.Catalog.BrandModelMaxNameLength)]
    public string Name { get; set; } = string.Empty;
}
