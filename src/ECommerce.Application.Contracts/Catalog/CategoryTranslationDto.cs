using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

/// <summary>
/// Translation item for category localized fields.
/// </summary>
public class CategoryTranslationDto : INameTranslationDto
{
    [Required]
    public string Language { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.Catalog.CategoryMaxNameLength)]
    public string Name { get; set; } = string.Empty;
}
