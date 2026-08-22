using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

public class CreateProductTypeDto
{
    [Required]
    [StringLength(ECommerceConsts.Catalog.ProductTypeMaxCodeLength)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.Catalog.ProductTypeMaxNameLength)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public List<ProductTypeTranslationDto> Translations { get; set; } = new();
}
