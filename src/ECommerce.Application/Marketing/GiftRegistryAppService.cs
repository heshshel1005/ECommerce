using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Cart;
using ECommerce.Catalog;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Marketing;

[Volo.Abp.RemoteService(IsEnabled = false)]
public class GiftRegistryAppService : ECommerceAppService, IGiftRegistryAppService
{
    private readonly IRepository<GiftRegistry, Guid> _registryRepository;
    private readonly IRepository<GiftRegistryItem, Guid> _itemRepository;
    private readonly IRepository<GiftRegistryClaim, Guid> _claimRepository;
    private readonly IRepository<ProductVariant, Guid> _variantRepository;
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly ICartAppService _cartAppService;
    private readonly IInventoryValidationAppService _inventoryValidation;

    public GiftRegistryAppService(
        IRepository<GiftRegistry, Guid> registryRepository,
        IRepository<GiftRegistryItem, Guid> itemRepository,
        IRepository<GiftRegistryClaim, Guid> claimRepository,
        IRepository<ProductVariant, Guid> variantRepository,
        IRepository<Product, Guid> productRepository,
        ICartAppService cartAppService,
        IInventoryValidationAppService inventoryValidation)
    {
        _registryRepository = registryRepository;
        _itemRepository = itemRepository;
        _claimRepository = claimRepository;
        _variantRepository = variantRepository;
        _productRepository = productRepository;
        _cartAppService = cartAppService;
        _inventoryValidation = inventoryValidation;
    }

    [AllowAnonymous]
    public async Task<GiftRegistryDto?> GetBySlugAsync(string slug)
    {
        var slugNorm = slug?.Trim().ToLowerInvariant() ?? "";
        if (string.IsNullOrEmpty(slugNorm)) return null;
        var registry = await _registryRepository.FirstOrDefaultAsync(r => r.Slug == slugNorm && r.IsPublic);
        if (registry == null) return null;
        return await BuildRegistryDtoAsync(registry);
    }

    [AllowAnonymous]
    public async Task ClaimAsync(ClaimRegistryItemDto input)
    {
        var item = await _itemRepository.GetAsync(input.GiftRegistryItemId);
        if (item.QuantityRemaining < input.Quantity)
            throw new Volo.Abp.BusinessException("ECommerce:GiftRegistryInsufficientQuantity")
                .WithData("Requested", input.Quantity).WithData("Remaining", item.QuantityRemaining);

        var claim = new GiftRegistryClaim(
            GuidGenerator.Create(),
            item.Id,
            input.Quantity,
            CurrentUser.Id,
            input.ClaimantName,
            input.Message);
        await _claimRepository.InsertAsync(claim);
        item.QuantityClaimed += input.Quantity;
        await _itemRepository.UpdateAsync(item);

        if (input.AddToCart && CurrentUser.Id.HasValue)
        {
            await _inventoryValidation.ValidateVariantAvailabilityAsync(item.ProductVariantId, input.Quantity);
            await _cartAppService.AddItemAsync(new AddCartItemDto { ProductVariantId = item.ProductVariantId, Quantity = input.Quantity }, null);
        }
    }

    [Authorize]
    public async Task<GiftRegistryDto> CreateAsync(CreateGiftRegistryDto input)
    {
        var userId = CurrentUser.Id ?? throw new Volo.Abp.Authorization.AbpAuthorizationException("ECommerce:LoginRequired");
        var slug = input.Slug.Trim().ToLowerInvariant();
        var existing = await _registryRepository.FirstOrDefaultAsync(r => r.Slug == slug);
        if (existing != null)
            throw new Volo.Abp.BusinessException("ECommerce:GiftRegistrySlugExists").WithData("Slug", slug);

        var registry = new GiftRegistry(GuidGenerator.Create(), userId, input.Title, slug, input.EventDate);
        await _registryRepository.InsertAsync(registry);
        return await BuildRegistryDtoAsync(registry);
    }

    [Authorize]
    public async Task<List<GiftRegistryDto>> GetMyRegistriesAsync()
    {
        var userId = CurrentUser.Id ?? throw new Volo.Abp.Authorization.AbpAuthorizationException("ECommerce:LoginRequired");
        var list = await _registryRepository.GetListAsync(r => r.OwnerUserId == userId);
        var result = new List<GiftRegistryDto>();
        foreach (var r in list)
            result.Add(await BuildRegistryDtoAsync(r));
        return result;
    }

    [Authorize]
    public async Task<GiftRegistryDto> AddItemAsync(Guid giftRegistryId, AddGiftRegistryItemDto input)
    {
        var registry = await _registryRepository.GetAsync(giftRegistryId);
        if (registry.OwnerUserId != CurrentUser.Id)
            throw new Volo.Abp.Authorization.AbpAuthorizationException("ECommerce:GiftRegistryAccessDenied");

        var existing = await _itemRepository.FirstOrDefaultAsync(x =>
            x.GiftRegistryId == giftRegistryId && x.ProductVariantId == input.ProductVariantId);
        if (existing != null)
        {
            existing.DesiredQuantity += input.DesiredQuantity;
            await _itemRepository.UpdateAsync(existing);
        }
        else
        {
            var item = new GiftRegistryItem(
                GuidGenerator.Create(),
                giftRegistryId,
                input.ProductVariantId,
                input.DesiredQuantity,
                input.Note);
            await _itemRepository.InsertAsync(item);
        }
        return await BuildRegistryDtoAsync(registry);
    }

    [Authorize]
    public async Task<GiftRegistryDto> RemoveItemAsync(Guid giftRegistryId, Guid giftRegistryItemId)
    {
        var registry = await _registryRepository.GetAsync(giftRegistryId);
        if (registry.OwnerUserId != CurrentUser.Id)
            throw new Volo.Abp.Authorization.AbpAuthorizationException("ECommerce:GiftRegistryAccessDenied");
        var item = await _itemRepository.FirstOrDefaultAsync(x => x.Id == giftRegistryItemId && x.GiftRegistryId == giftRegistryId);
        if (item != null)
            await _itemRepository.DeleteAsync(item);
        return await BuildRegistryDtoAsync(registry);
    }

    private async Task<GiftRegistryDto> BuildRegistryDtoAsync(GiftRegistry registry)
    {
        var items = await _itemRepository.GetListAsync(x => x.GiftRegistryId == registry.Id);
        if (items.Count == 0)
            return new GiftRegistryDto { Id = registry.Id, Title = registry.Title, Slug = registry.Slug, EventDate = registry.EventDate };

        var variantIds = items.Select(x => x.ProductVariantId).Distinct().ToList();
        var variants = await _variantRepository.GetListAsync(v => variantIds.Contains(v.Id));
        var productIds = variants.Select(v => v.ProductId).Distinct().ToList();
        var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));
        var variantMap = variants.ToDictionary(v => v.Id);
        var productMap = products.ToDictionary(p => p.Id);

        var itemDtos = items.Select(item =>
        {
            variantMap.TryGetValue(item.ProductVariantId, out var variant);
            var product = variant != null && productMap.TryGetValue(variant.ProductId, out var p) ? p : null;
            return new GiftRegistryItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductId = variant?.ProductId ?? Guid.Empty,
                ProductName = product?.Name ?? "",
                Sku = variant?.Sku ?? "",
                Price = variant?.Price,
                DesiredQuantity = item.DesiredQuantity,
                QuantityClaimed = item.QuantityClaimed,
                QuantityRemaining = item.QuantityRemaining,
                Note = item.Note
            };
        }).ToList();

        return new GiftRegistryDto
        {
            Id = registry.Id,
            Title = registry.Title,
            Slug = registry.Slug,
            EventDate = registry.EventDate,
            Items = itemDtos
        };
    }
}
