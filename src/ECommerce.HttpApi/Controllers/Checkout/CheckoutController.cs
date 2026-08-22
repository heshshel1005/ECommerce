using System;
using System.Threading.Tasks;
using ECommerce.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Checkout;

[Route("api/app/checkout")]
[Area("app")]
public class CheckoutController : ECommerceController
{
    private readonly ICheckoutAppService _appService;

    public CheckoutController(ICheckoutAppService appService)
    {
        _appService = appService;
    }

    /// <summary>
    /// Get checkout summary (cart, optional coupon discount, shipping options, tax). For guests pass guestCartId (query or header X-Guest-Cart-Id). Pass couponCode to preview discount.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("summary")]
    public async Task<CheckoutSummaryDto> GetSummaryAsync([FromQuery] Guid? guestCartId = null, [FromQuery] string? couponCode = null)
    {
        var guestId = ResolveGuestCartId(guestCartId);
        return await _appService.GetSummaryAsync(guestId, couponCode);
    }

    /// <summary>
    /// Submit order from cart. Creates order with contact and addresses, clears cart. For guests pass guestCartId.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("submit")]
    public async Task<SubmitCheckoutResultDto> SubmitOrderAsync([FromBody] SubmitCheckoutDto input, [FromQuery] Guid? guestCartId = null)
    {
        var guestId = ResolveGuestCartId(guestCartId);
        return await _appService.SubmitOrderAsync(input, guestId);
    }

    private Guid? ResolveGuestCartId(Guid? guestCartId)
    {
        if (guestCartId != null && guestCartId != Guid.Empty)
            return guestCartId;
        if (Request.Headers.TryGetValue("X-Guest-Cart-Id", out var hv) && !StringValues.IsNullOrEmpty(hv))
        {
            var s = hv.ToString();
            if (Guid.TryParse(s, out var g))
                return g;
        }
        return null;
    }
}
