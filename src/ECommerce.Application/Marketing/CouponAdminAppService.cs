using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Marketing;

[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize("ECommerce.Administration")]
public class CouponAdminAppService : ECommerceAppService, ICouponAdminAppService
{
    private readonly IRepository<Coupon, Guid> _couponRepository;

    public CouponAdminAppService(IRepository<Coupon, Guid> couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<CouponDto> CreateAsync(CreateCouponDto input)
    {
        var existing = await _couponRepository.FirstOrDefaultAsync(c =>
            c.Code == input.Code.Trim().ToUpperInvariant());
        if (existing != null)
            throw new Volo.Abp.BusinessException("ECommerce:CouponCodeExists").WithData("Code", input.Code);

        var coupon = new Coupon(
            GuidGenerator.Create(),
            input.Code,
            input.Type,
            input.Value,
            input.MinOrderAmount,
            input.ValidFrom,
            input.ValidTo,
            input.TotalUsageLimit,
            input.PerUserUsageLimit);
        if (!input.IsActive)
            coupon.IsActive = false; // Coupon doesn't have SetActive; we'd need to add it or use a new entity. For now we'll add IsActive via reflection or skip - actually the ctor doesn't set IsActive. So the entity has IsActive = true by default. So we need to set it after creation. The entity has a public setter for IsActive. So we can do: after InsertAsync we could load and set, or add a constructor that takes isActive. Simpler: just set coupon.IsActive = input.IsActive after creating - the property has set.
        coupon.IsActive = input.IsActive;
        await _couponRepository.InsertAsync(coupon);
        return MapToDto(coupon);
    }

    public async Task<PagedResultDto<CouponDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _couponRepository.GetQueryableAsync();
        var total = await AsyncExecuter.CountAsync(query);
        var sort = input.Sorting ?? "Code";
        query = query.OrderBy(x => x.Code);
        var skip = input.SkipCount;
        var take = input.MaxResultCount > 0 ? input.MaxResultCount : 10;
        var list = await AsyncExecuter.ToListAsync(query.Skip(skip).Take(take));
        return new PagedResultDto<CouponDto>(total, list.Select(MapToDto).ToList());
    }

    public async Task<CouponDto?> GetByCodeAsync(string code)
    {
        var coupon = await _couponRepository.FirstOrDefaultAsync(c =>
            c.Code == code.Trim().ToUpperInvariant());
        return coupon == null ? null : MapToDto(coupon);
    }

    private static CouponDto MapToDto(Coupon c)
    {
        return new CouponDto
        {
            Id = c.Id,
            Code = c.Code,
            Type = (int)c.Type,
            Value = c.Value,
            MinOrderAmount = c.MinOrderAmount,
            ValidFrom = c.ValidFrom,
            ValidTo = c.ValidTo,
            TotalUsageLimit = c.TotalUsageLimit,
            PerUserUsageLimit = c.PerUserUsageLimit,
            IsActive = c.IsActive
        };
    }
}
