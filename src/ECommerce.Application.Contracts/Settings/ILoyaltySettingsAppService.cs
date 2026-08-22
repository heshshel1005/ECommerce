using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Settings;

public interface ILoyaltySettingsAppService : IApplicationService
{
    Task<LoyaltySettingsDto> GetAsync();
    Task UpdateAsync(UpdateLoyaltySettingsDto input);
}
