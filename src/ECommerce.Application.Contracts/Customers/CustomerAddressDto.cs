using System;
using Volo.Abp.Application.Dtos;

namespace ECommerce.Customers;

public class CustomerAddressDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public bool IsDefaultShipping { get; set; }
    public bool IsDefaultBilling { get; set; }
}
