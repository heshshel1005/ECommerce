using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

public class ProductTypeTranslationDto
{
    [Required]
    public string Language { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.Catalog.ProductTypeMaxNameLength)]
    public string Name { get; set; } = string.Empty;
}
