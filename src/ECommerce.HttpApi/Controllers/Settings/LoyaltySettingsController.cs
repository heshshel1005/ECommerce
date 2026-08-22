using System.Threading.Tasks;
using ECommerce.Settings;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Settings;

[Route("api/app/loyalty-settings")]
[Area("app")]
public class LoyaltySettingsController : ECommerceController
{
    private readonly ILoyaltySettingsAppService _appService;

    public LoyaltySettingsController(ILoyaltySettingsAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<LoyaltySettingsDto> GetAsync()
    {
        return await _appService.GetAsync();
    }

    [HttpPost]
    public async Task UpdateAsync([FromBody] UpdateLoyaltySettingsDto input)
    {
        await _appService.UpdateAsync(input);
    }
}
