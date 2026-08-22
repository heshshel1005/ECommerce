using System;
using System.Threading.Tasks;
using ECommerce.Cart;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Cart;

/// <summary>
/// Admin: release reserved inventory for carts inactive longer than a given period (cart reserve timeout).
/// </summary>
[Route("api/app/cart-admin")]
[Area("app")]
public class CartReservationExpiryController : ECommerceController
{
    private readonly ICartReservationExpiryService _expiryService;

    public CartReservationExpiryController(ICartReservationExpiryService expiryService)
    {
        _expiryService = expiryService;
    }

    /// <summary>
    /// Release reservations for carts not modified in the last <paramref name="olderThanMinutes"/> minutes.
    /// </summary>
    [HttpPost("release-expired-reservations")]
    public async Task<int> ReleaseExpiredReservationsAsync([FromQuery] int olderThanMinutes = 30)
    {
        if (olderThanMinutes <= 0)
            olderThanMinutes = 30;
        return await _expiryService.ReleaseExpiredReservationsAsync(TimeSpan.FromMinutes(olderThanMinutes));
    }
}
