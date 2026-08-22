using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Customers;

/// <summary>
/// Customer profile and addresses for the current user (My Account / Profile).
/// </summary>
public interface ICustomerProfileAppService : IApplicationService
{
    Task<CustomerProfileDto> GetMyProfileAsync();
    Task<CustomerProfileDto> UpdateMyProfileAsync(UpdateCustomerProfileDto input);
    Task<List<CustomerAddressDto>> GetMyAddressesAsync();
    Task<CustomerAddressDto> CreateAddressAsync(CreateUpdateCustomerAddressDto input);
    Task<CustomerAddressDto> UpdateAddressAsync(Guid id, CreateUpdateCustomerAddressDto input);
    Task DeleteAddressAsync(Guid id);
}
