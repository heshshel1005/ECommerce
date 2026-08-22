using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Marketing;

[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize("ECommerce.Administration")]
public class RedemptionRuleAdminAppService : ECommerceAppService, IRedemptionRuleAdminAppService
{
    private readonly IRepository<RedemptionRule, Guid> _repository;

    public RedemptionRuleAdminAppService(IRepository<RedemptionRule, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<RedemptionRuleDto> CreateAsync(CreateRedemptionRuleDto input)
    {
        var rule = new RedemptionRule(
            GuidGenerator.Create(),
            input.Name,
            (RedemptionRuleType)input.Type,
            input.PointsRequired,
            input.Value,
            input.MinOrderAmount);
        await _repository.InsertAsync(rule);
        return MapToDto(rule);
    }

    public async Task<RedemptionRuleDto> UpdateAsync(Guid id, UpdateRedemptionRuleDto input)
    {
        var rule = await _repository.GetAsync(id);
        rule.Name = input.Name;
        rule.PointsRequired = input.PointsRequired;
        rule.Value = input.Value;
        rule.MinOrderAmount = input.MinOrderAmount;
        rule.IsActive = input.IsActive;
        await _repository.UpdateAsync(rule);
        return MapToDto(rule);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<List<RedemptionRuleDto>> GetListAsync()
    {
        var list = await _repository.GetListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<RedemptionRuleDto?> GetAsync(Guid id)
    {
        var rule = await _repository.FindAsync(id);
        return rule == null ? null : MapToDto(rule);
    }

    private static RedemptionRuleDto MapToDto(RedemptionRule r)
    {
        return new RedemptionRuleDto
        {
            Id = r.Id,
            Name = r.Name,
            Type = (int)r.Type,
            PointsRequired = r.PointsRequired,
            Value = r.Value,
            MinOrderAmount = r.MinOrderAmount,
            IsActive = r.IsActive,
        };
    }
}
