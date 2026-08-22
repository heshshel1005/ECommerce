using System.ComponentModel.DataAnnotations;

namespace ECommerce.Account;

/// <summary>
/// Input for a single address (shipping or billing).
/// </summary>
public class AddressInputDto
{
    [StringLength(64)]
    public string Label { get; set; } = string.Empty;

    [Required]
    [StringLength(512)]
    public string Street { get; set; } = string.Empty;

    [StringLength(128)]
    public string? City { get; set; }

    [StringLength(128)]
    public string? Region { get; set; }

    [StringLength(32)]
    public string? PostalCode { get; set; }

    [StringLength(128)]
    public string? Country { get; set; }
}
