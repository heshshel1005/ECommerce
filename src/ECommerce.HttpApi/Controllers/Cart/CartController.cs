using System;
using System.Threading.Tasks;
using ECommerce.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Cart;

/// <summary>
/// Cart API: get cart, add/update/remove items, merge guest cart on login.
/// Supports guest (anonymous) and authenticated users.
/// </summary>
[Route("api/app/cart")]
[Area("app")]
public class CartController : ECommerceController
{
    private readonly ICartAppService _appService;

    public CartController(ICartAppService appService)
    {
        _appService = appService;
    }

    /// <summary>
    /// Get the current cart. For guests pass guestCartId (query or header X-Guest-Cart-Id). For authenticated users optional guestCartId merges and returns the merged cart.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<CartDto> GetCartAsync([FromQuery] Guid? guestCartId = null)
    {
        var guestId = ResolveGuestCartId(guestCartId);
        return await _appService.GetCartAsync(guestId);
    }

    /// <summary>
    /// Add a product variant to the cart. Validates stock. For guests pass guestCartId.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("items")]
    public async Task<CartDto> AddItemAsync([FromBody] AddCartItemDto input, [FromQuery] Guid? guestCartId = null)
    {
        var guestId = ResolveGuestCartId(guestCartId);
        return await _appService.AddItemAsync(input, guestId);
    }

    /// <summary>
    /// Update a cart line quantity. Validates stock.
    /// </summary>
    [AllowAnonymous]
    [HttpPut("items/{cartItemId}")]
    public async Task<CartDto> UpdateItemAsync(Guid cartItemId, [FromBody] UpdateCartItemDto input, [FromQuery] Guid? guestCartId = null)
    {
        var guestId = ResolveGuestCartId(guestCartId);
        return await _appService.UpdateItemAsync(cartItemId, input, guestId);
    }

    /// <summary>
    /// Remove a line from the cart.
    /// </summary>
    [AllowAnonymous]
    [HttpDelete("items/{cartItemId}")]
    public async Task<CartDto> RemoveItemAsync(Guid cartItemId, [FromQuery] Guid? guestCartId = null)
    {
        var guestId = ResolveGuestCartId(guestCartId);
        return await _appService.RemoveItemAsync(cartItemId, guestId);
    }

    /// <summary>
    /// Merge the guest cart into the current user's cart. Call after login. Requires authentication.
    /// </summary>
    [Authorize]
    [HttpPost("merge-guest")]
    public async Task<CartDto> MergeGuestCartAsync([FromBody] MergeGuestCartRequest request)
    {
        return await _appService.MergeGuestCartAsync(request.GuestCartId);
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

/// <summary>
/// Request body for merging guest cart after login.
/// </summary>
public class MergeGuestCartRequest
{
    public Guid GuestCartId { get; set; }
}
