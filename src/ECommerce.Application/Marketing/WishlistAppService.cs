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
public class WishlistAppService : ECommerceAppService, IWishlistAppService
{
    private readonly IRepository<Wishlist, Guid> _wishlistRepository;
    private readonly IRepository<WishlistItem, Guid> _wishlistItemRepository;
    private readonly IRepository<ProductVariant, Guid> _variantRepository;
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IRepository<Inventory, Guid> _inventoryRepository;
    private readonly ICartAppService _cartAppService;
    private readonly IInventoryValidationAppService _inventoryValidation;

    public WishlistAppService(
        IRepository<Wishlist, Guid> wishlistRepository,
        IRepository<WishlistItem, Guid> wishlistItemRepository,
        IRepository<ProductVariant, Guid> variantRepository,
        IRepository<Product, Guid> productRepository,
        IRepository<Inventory, Guid> inventoryRepository,
        ICartAppService cartAppService,
        IInventoryValidationAppService inventoryValidation)
    {
        _wishlistRepository = wishlistRepository;
        _wishlistItemRepository = wishlistItemRepository;
        _variantRepository = variantRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _cartAppService = cartAppService;
        _inventoryValidation = inventoryValidation;
    }

    [Authorize]
    public async Task<WishlistDto> GetListAsync()
    {
        var userId = CurrentUser.Id!.Value;
        var wishlist = await GetOrCreateWishlistAsync(userId);
        return await BuildWishlistDtoAsync(wishlist);
    }

    [Authorize]
    public async Task<WishlistDto> AddItemAsync(Guid productVariantId)
    {
        var userId = CurrentUser.Id!.Value;
        var wishlist = await GetOrCreateWishlistAsync(userId);
        var existing = await _wishlistItemRepository.FirstOrDefaultAsync(x =>
            x.WishlistId == wishlist.Id && x.ProductVariantId == productVariantId);
        if (existing == null)
        {
            var item = new WishlistItem(GuidGenerator.Create(), wishlist.Id, productVariantId);
            await _wishlistItemRepository.InsertAsync(item);
        }
        return await BuildWishlistDtoAsync(wishlist);
    }

    [Authorize]
    public async Task<WishlistDto> RemoveItemAsync(Guid wishlistItemId)
    {
        var userId = CurrentUser.Id!.Value;
        var wishlist = await GetOrCreateWishlistAsync(userId);
        var item = await _wishlistItemRepository.FirstOrDefaultAsync(x => x.Id == wishlistItemId && x.WishlistId == wishlist.Id);
        if (item != null)
            await _wishlistItemRepository.DeleteAsync(item);
        return await BuildWishlistDtoAsync(wishlist);
    }

    [Authorize]
    public async Task AddToCartAsync(Guid wishlistItemId)
    {
        var userId = CurrentUser.Id!.Value;
        var item = await _wishlistItemRepository.FirstOrDefaultAsync(x => x.Id == wishlistItemId);
        if (item == null)
            throw new Volo.Abp.BusinessException("ECommerce:WishlistItemNotFound").WithData("Id", wishlistItemId);
        var wishlist = await _wishlistRepository.GetAsync(item.WishlistId);
        if (wishlist.UserId != userId)
            throw new Volo.Abp.Authorization.AbpAuthorizationException("ECommerce:WishlistAccessDenied");
        await _inventoryValidation.ValidateVariantAvailabilityAsync(item.ProductVariantId, 1);
        await _cartAppService.AddItemAsync(new AddCartItemDto { ProductVariantId = item.ProductVariantId, Quantity = 1 }, null);
    }

    private async Task<Wishlist> GetOrCreateWishlistAsync(Guid userId)
    {
        var wishlist = await _wishlistRepository.FirstOrDefaultAsync(x => x.UserId == userId);
        if (wishlist != null) return wishlist;
        wishlist = new Wishlist(GuidGenerator.Create(), userId);
        await _wishlistRepository.InsertAsync(wishlist);
        return wishlist;
    }

    private async Task<WishlistDto> BuildWishlistDtoAsync(Wishlist wishlist)
    {
        var items = await _wishlistItemRepository.GetListAsync(x => x.WishlistId == wishlist.Id);
        if (items.Count == 0)
            return new WishlistDto { Id = wishlist.Id };

        var variantIds = items.Select(x => x.ProductVariantId).Distinct().ToList();
        var variants = await _variantRepository.GetListAsync(v => variantIds.Contains(v.Id));
        var productIds = variants.Select(v => v.ProductId).Distinct().ToList();
        var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));
        var inventories = await _inventoryRepository.GetListAsync(i => variantIds.Contains(i.ProductVariantId));
        var productMap = products.ToDictionary(p => p.Id);
        var variantMap = variants.ToDictionary(v => v.Id);
        var invMap = inventories.ToDictionary(i => i.ProductVariantId);

        var itemDtos = new List<WishlistItemDto>();
        foreach (var item in items)
        {
            if (!variantMap.TryGetValue(item.ProductVariantId, out var variant)) continue;
            var product = productMap.GetValueOrDefault(variant.ProductId);
            invMap.TryGetValue(item.ProductVariantId, out var inv);
            itemDtos.Add(new WishlistItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductId = variant.ProductId,
                ProductName = product?.Name ?? "",
                Sku = variant.Sku ?? "",
                Price = variant.Price,
                AvailableQuantity = inv?.AvailableQuantity
            });
        }
        return new WishlistDto { Id = wishlist.Id, Items = itemDtos };
    }
}
