using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Marketing;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Marketing;

[Route("api/app/redemption-rule-admin")]
[Area("app")]
public class RedemptionRuleAdminController : ECommerceController
{
    private readonly IRedemptionRuleAdminAppService _appService;

    public RedemptionRuleAdminController(IRedemptionRuleAdminAppService appService)
    {
        _appService = appService;
    }

    [HttpPost]
    public async Task<RedemptionRuleDto> CreateAsync([FromBody] CreateRedemptionRuleDto input)
    {
        return await _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<RedemptionRuleDto> UpdateAsync(Guid id, [FromBody] UpdateRedemptionRuleDto input)
    {
        return await _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _appService.DeleteAsync(id);
    }

    [HttpGet]
    public async Task<List<RedemptionRuleDto>> GetListAsync()
    {
        return await _appService.GetListAsync();
    }

    [HttpGet("{id}")]
    public async Task<RedemptionRuleDto?> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }
}
