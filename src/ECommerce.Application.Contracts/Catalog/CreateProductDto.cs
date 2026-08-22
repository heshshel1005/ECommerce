using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

/// <summary>
/// Input for creating a product with variants and inventory.
/// </summary>
public class CreateProductDto
{
    [Required]
    [StringLength(ECommerceConsts.Catalog.ProductMaxProductNumberLength)]
    public string ProductNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.Catalog.ProductMaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(ECommerceConsts.Catalog.ProductMaxDescriptionLength)]
    public string? Description { get; set; }

    public Guid? CategoryId { get; set; }

    [Required]
    public Guid BrandId { get; set; }

    public Guid? ModelId { get; set; }

    public Guid? ProductTypeId { get; set; }

    public Dictionary<string, object?> DynamicAttributes { get; set; } = new();

    public bool IsPublished { get; set; }

    public List<ProductTranslationDto> Translations { get; set; } = new();

    public List<CreateProductVariantDto> Variants { get; set; } = new();
}
