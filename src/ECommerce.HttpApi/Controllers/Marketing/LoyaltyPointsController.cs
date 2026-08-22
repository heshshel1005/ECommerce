using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Marketing;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Marketing;

[Route("api/app/loyalty")]
[Area("app")]
public class LoyaltyPointsController : ECommerceController
{
    private readonly ILoyaltyPointsAppService _appService;

    public LoyaltyPointsController(ILoyaltyPointsAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("my-points")]
    public async Task<CustomerPointsDto> GetMyPointsAsync()
    {
        return await _appService.GetMyPointsAsync();
    }

    [HttpGet("redemption-rules")]
    public async Task<List<RedemptionRuleDto>> GetActiveRedemptionRulesAsync()
    {
        return await _appService.GetActiveRedemptionRulesAsync();
    }
}
