using System.Threading.Tasks;
using ECommerce.Settings;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.SettingManagement;

namespace ECommerce.Application.Settings;

public static class LoyaltySettingNames
{
    public const string PointsPerCurrencyUnit = "ECommerce.Loyalty.PointsPerCurrencyUnit";
}

[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize("ECommerce.Administration")]
public class LoyaltySettingsAppService : ECommerceAppService, ILoyaltySettingsAppService
{
    private readonly ISettingManager _settingManager;

    public LoyaltySettingsAppService(ISettingManager settingManager)
    {
        _settingManager = settingManager;
    }

    public async Task<LoyaltySettingsDto> GetAsync()
    {
        var value = await _settingManager.GetOrNullGlobalAsync(LoyaltySettingNames.PointsPerCurrencyUnit);
        return new LoyaltySettingsDto
        {
            PointsPerCurrencyUnit = value ?? "1",
        };
    }

    public async Task UpdateAsync(UpdateLoyaltySettingsDto input)
    {
        await _settingManager.SetGlobalAsync(
            LoyaltySettingNames.PointsPerCurrencyUnit,
            input.PointsPerCurrencyUnit?.Trim() ?? "1");
    }
}
