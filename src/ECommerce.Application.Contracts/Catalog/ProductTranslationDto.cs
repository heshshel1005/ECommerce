using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

/// <summary>
/// Translation item for product localized fields.
/// </summary>
public class ProductTranslationDto : INameDescriptionTranslationDto
{
    [Required]
    public string Language { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.Catalog.ProductMaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(ECommerceConsts.Catalog.ProductMaxDescriptionLength)]
    public string? Description { get; set; }
}
