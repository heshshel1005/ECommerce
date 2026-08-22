using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Customers;

[Route("api/app/customer-profile")]
[Area("app")]
[Authorize]
public class CustomerProfileController : ECommerceController
{
    private readonly ICustomerProfileAppService _appService;

    public CustomerProfileController(ICustomerProfileAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<CustomerProfileDto> GetMyProfileAsync()
    {
        return await _appService.GetMyProfileAsync();
    }

    [HttpPut]
    public async Task<CustomerProfileDto> UpdateMyProfileAsync([FromBody] UpdateCustomerProfileDto input)
    {
        return await _appService.UpdateMyProfileAsync(input);
    }

    [HttpGet("addresses")]
    public async Task<List<CustomerAddressDto>> GetMyAddressesAsync()
    {
        return await _appService.GetMyAddressesAsync();
    }

    [HttpPost("addresses")]
    public async Task<CustomerAddressDto> CreateAddressAsync([FromBody] CreateUpdateCustomerAddressDto input)
    {
        return await _appService.CreateAddressAsync(input);
    }

    [HttpPut("addresses/{id}")]
    public async Task<CustomerAddressDto> UpdateAddressAsync(Guid id, [FromBody] CreateUpdateCustomerAddressDto input)
    {
        return await _appService.UpdateAddressAsync(id, input);
    }

    [HttpDelete("addresses/{id}")]
    public async Task DeleteAddressAsync(Guid id)
    {
        await _appService.DeleteAddressAsync(id);
    }
}
