using System.ComponentModel.DataAnnotations;

namespace ECommerce.Account;

/// <summary>
/// Full customer subscription input: account, contact, default shipping address, optional billing.
/// </summary>
public class CustomerRegisterDto
{
    // Account (from ABP RegisterDto)
    [Required]
    [StringLength(256)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string EmailAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(128)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? AppName { get; set; }
    public string? ReturnUrl { get; set; }
    public string? ReturnUrlHash { get; set; }

    // Contact
    [Required]
    [StringLength(256)]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(32)]
    public string? PhoneNumber { get; set; }

    // Default shipping address (required)
    [Required]
    public AddressInputDto ShippingAddress { get; set; } = null!;

    // Optional billing address (null = same as shipping)
    public AddressInputDto? BillingAddress { get; set; }
}
