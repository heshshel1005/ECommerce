using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ECommerce.Marketing;

public interface ICouponAdminAppService : IApplicationService
{
    Task<CouponDto> CreateAsync(CreateCouponDto input);
    Task<PagedResultDto<CouponDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<CouponDto?> GetByCodeAsync(string code);
}
