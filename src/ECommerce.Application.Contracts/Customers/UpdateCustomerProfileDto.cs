using System.ComponentModel.DataAnnotations;

namespace ECommerce.Customers;

public class UpdateCustomerProfileDto
{
    [Required]
    [StringLength(ECommerceConsts.CustomerProfile.MaxDisplayNameLength)]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(ECommerceConsts.CustomerProfile.MaxPhoneNumberLength)]
    public string? PhoneNumber { get; set; }
}
