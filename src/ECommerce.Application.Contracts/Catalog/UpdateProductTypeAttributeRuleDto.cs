using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Catalog;

public class UpdateProductTypeAttributeRuleDto
{
    [Required]
    public Guid AttributeDefinitionId { get; set; }
    public int DisplayOrder { get; set; }
}
