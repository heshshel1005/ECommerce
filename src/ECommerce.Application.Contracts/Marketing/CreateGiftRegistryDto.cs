using System.ComponentModel.DataAnnotations;

namespace ECommerce.Marketing;

public class CreateGiftRegistryDto
{
    [Required]
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string Slug { get; set; } = string.Empty;

    public System.DateTime? EventDate { get; set; }
}
