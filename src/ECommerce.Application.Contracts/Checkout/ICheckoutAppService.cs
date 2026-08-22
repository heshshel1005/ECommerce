using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Checkout;

/// <summary>
/// Checkout: get summary (cart + shipping options + tax), submit order from cart.
/// API is exposed via CheckoutController (to support X-Guest-Cart-Id header).
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
public interface ICheckoutAppService : IApplicationService
{
    /// <summary>
    /// Gets checkout summary for the current cart: items, subtotal, optional coupon discount, shipping options, tax. For guests pass guestCartId.
    /// </summary>
    Task<CheckoutSummaryDto> GetSummaryAsync(Guid? guestCartId = null, string? couponCode = null);

    /// <summary>
    /// Submits the order from the current cart. Validates stock, creates order record, clears cart. For guests pass guestCartId.
    /// Returns the created order id.
    /// </summary>
    Task<SubmitCheckoutResultDto> SubmitOrderAsync(SubmitCheckoutDto input, Guid? guestCartId = null);
}
