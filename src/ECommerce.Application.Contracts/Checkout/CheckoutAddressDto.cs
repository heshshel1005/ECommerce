using System.ComponentModel.DataAnnotations;

namespace ECommerce.Checkout;

/// <summary>
/// Address for checkout (shipping or billing).
/// </summary>
public class CheckoutAddressDto
{
    [Required]
    [StringLength(512)]
    public string Street { get; set; } = string.Empty;

    [StringLength(512)]
    public string? Street2 { get; set; }

    [StringLength(128)]
    public string? City { get; set; }

    [StringLength(128)]
    public string? Region { get; set; }

    [StringLength(32)]
    public string? PostalCode { get; set; }

    [StringLength(128)]
    public string? Country { get; set; }

    [StringLength(500)]
    public string? DeliveryInstructions { get; set; }
}
