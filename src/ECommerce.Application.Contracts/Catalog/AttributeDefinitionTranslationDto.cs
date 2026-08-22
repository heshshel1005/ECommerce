using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

/// <summary>
/// Translation item for <see cref="AttributeDefinition"/> localized display name and description.
/// </summary>
public class AttributeDefinitionTranslationDto : INameDescriptionTranslationDto
{
    [Required]
    [StringLength(ECommerceConsts.Catalog.TranslationLanguageMaxLength)]
    public string Language { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.Catalog.AttributeDefinitionTranslationDisplayNameMaxLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(ECommerceConsts.Catalog.AttributeDefinitionTranslationDescriptionMaxLength)]
    public string? Description { get; set; }
}
