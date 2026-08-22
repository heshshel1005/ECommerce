using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Marketing;

/// <summary>
/// Admin: CRUD for redemption rules (points for discount / free shipping).
/// </summary>
public interface IRedemptionRuleAdminAppService : IApplicationService
{
    Task<RedemptionRuleDto> CreateAsync(CreateRedemptionRuleDto input);
    Task<RedemptionRuleDto> UpdateAsync(Guid id, UpdateRedemptionRuleDto input);
    Task DeleteAsync(Guid id);
    Task<List<RedemptionRuleDto>> GetListAsync();
    Task<RedemptionRuleDto?> GetAsync(Guid id);
}
