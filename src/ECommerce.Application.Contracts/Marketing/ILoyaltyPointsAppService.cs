using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Marketing;

/// <summary>
/// Customer: get my points balance and redemption options. Points are awarded on order confirmation.
/// </summary>
public interface ILoyaltyPointsAppService : IApplicationService
{
    Task<CustomerPointsDto> GetMyPointsAsync();
    Task<List<RedemptionRuleDto>> GetActiveRedemptionRulesAsync();
}
