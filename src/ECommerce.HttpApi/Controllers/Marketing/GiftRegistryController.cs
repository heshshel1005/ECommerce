using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Marketing;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Marketing;

[Route("api/app/gift-registry")]
[Area("app")]
public class GiftRegistryController : ECommerceController
{
    private readonly IGiftRegistryAppService _appService;

    public GiftRegistryController(IGiftRegistryAppService appService)
    {
        _appService = appService;
    }

    /// <summary>Get a public registry by slug.</summary>
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    [HttpGet("by-slug/{slug}")]
    public async Task<GiftRegistryDto?> GetBySlugAsync(string slug)
    {
        return await _appService.GetBySlugAsync(slug);
    }

    /// <summary>Claim (reserve/purchase) an item. Optionally add to cart.</summary>
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    [HttpPost("claim")]
    public async Task ClaimAsync([FromBody] ClaimRegistryItemDto input)
    {
        await _appService.ClaimAsync(input);
    }

    [HttpPost]
    public async Task<GiftRegistryDto> CreateAsync([FromBody] CreateGiftRegistryDto input)
    {
        return await _appService.CreateAsync(input);
    }

    [HttpGet("my")]
    public async Task<List<GiftRegistryDto>> GetMyRegistriesAsync()
    {
        return await _appService.GetMyRegistriesAsync();
    }

    [HttpPost("{giftRegistryId}/items")]
    public async Task<GiftRegistryDto> AddItemAsync(Guid giftRegistryId, [FromBody] AddGiftRegistryItemDto input)
    {
        return await _appService.AddItemAsync(giftRegistryId, input);
    }

    [HttpDelete("{giftRegistryId}/items/{giftRegistryItemId}")]
    public async Task<GiftRegistryDto> RemoveItemAsync(Guid giftRegistryId, Guid giftRegistryItemId)
    {
        return await _appService.RemoveItemAsync(giftRegistryId, giftRegistryItemId);
    }
}
