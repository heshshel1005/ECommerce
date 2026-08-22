using System.ComponentModel.DataAnnotations;

namespace ECommerce.Customers;

public class CreateUpdateCustomerAddressDto
{
    [StringLength(ECommerceConsts.CustomerAddress.MaxLabelLength)]
    public string Label { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.CustomerAddress.MaxStreetLength)]
    public string Street { get; set; } = string.Empty;

    [StringLength(ECommerceConsts.CustomerAddress.MaxCityLength)]
    public string? City { get; set; }

    [StringLength(ECommerceConsts.CustomerAddress.MaxRegionLength)]
    public string? Region { get; set; }

    [StringLength(ECommerceConsts.CustomerAddress.MaxPostalCodeLength)]
    public string? PostalCode { get; set; }

    [StringLength(ECommerceConsts.CustomerAddress.MaxCountryLength)]
    public string? Country { get; set; }

    public bool IsDefaultShipping { get; set; }
    public bool IsDefaultBilling { get; set; }
}
