using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Marketing;

[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize]
public class LoyaltyPointsAppService : ECommerceAppService, ILoyaltyPointsAppService
{
    private readonly IRepository<CustomerPoints, Guid> _customerPointsRepository;
    private readonly IRepository<RedemptionRule, Guid> _redemptionRuleRepository;

    public LoyaltyPointsAppService(
        IRepository<CustomerPoints, Guid> customerPointsRepository,
        IRepository<RedemptionRule, Guid> redemptionRuleRepository)
    {
        _customerPointsRepository = customerPointsRepository;
        _redemptionRuleRepository = redemptionRuleRepository;
    }

    public async Task<CustomerPointsDto> GetMyPointsAsync()
    {
        var userId = CurrentUser.Id ?? throw new Volo.Abp.Authorization.AbpAuthorizationException("User must be logged in.");
        var cp = await _customerPointsRepository.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cp == null)
            return new CustomerPointsDto { UserId = userId, Balance = 0 };

        return new CustomerPointsDto
        {
            Id = cp.Id,
            UserId = cp.UserId,
            Balance = cp.Balance,
            Tier = cp.Tier,
        };
    }

    public async Task<List<RedemptionRuleDto>> GetActiveRedemptionRulesAsync()
    {
        var rules = await _redemptionRuleRepository.GetListAsync(r => r.IsActive);
        return rules.Select(r => new RedemptionRuleDto
        {
            Id = r.Id,
            Name = r.Name,
            Type = (int)r.Type,
            PointsRequired = r.PointsRequired,
            Value = r.Value,
            MinOrderAmount = r.MinOrderAmount,
            IsActive = r.IsActive,
        }).ToList();
    }
}
