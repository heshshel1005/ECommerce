using System;
using System.Threading.Tasks;
using ECommerce.Marketing;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Marketing;

[Route("api/app/coupon-admin")]
[Area("app")]
public class CouponAdminController : ECommerceController
{
    private readonly ICouponAdminAppService _appService;

    public CouponAdminController(ICouponAdminAppService appService)
    {
        _appService = appService;
    }

    [HttpPost]
    public async Task<CouponDto> CreateAsync([FromBody] CreateCouponDto input)
    {
        return await _appService.CreateAsync(input);
    }

    [HttpGet]
    public async Task<PagedResultDto<CouponDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return await _appService.GetListAsync(input);
    }

    [HttpGet("by-code/{code}")]
    public async Task<CouponDto?> GetByCodeAsync(string code)
    {
        return await _appService.GetByCodeAsync(code);
    }
}
