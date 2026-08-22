using System;
using System.Threading.Tasks;
using ECommerce.Marketing;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Marketing;

[Route("api/app/wishlist")]
[Area("app")]
public class WishlistController : ECommerceController
{
    private readonly IWishlistAppService _appService;

    public WishlistController(IWishlistAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<WishlistDto> GetListAsync()
    {
        return await _appService.GetListAsync();
    }

    [HttpPost("items")]
    public async Task<WishlistDto> AddItemAsync([FromQuery] Guid productVariantId)
    {
        return await _appService.AddItemAsync(productVariantId);
    }

    [HttpDelete("items/{wishlistItemId}")]
    public async Task<WishlistDto> RemoveItemAsync(Guid wishlistItemId)
    {
        return await _appService.RemoveItemAsync(wishlistItemId);
    }

    [HttpPost("items/{wishlistItemId}/add-to-cart")]
    public async Task AddToCartAsync(Guid wishlistItemId)
    {
        await _appService.AddToCartAsync(wishlistItemId);
    }
}
