using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Cart;

/// <summary>
/// Cart API: guest and authenticated; persistent for logged-in users; merge on login; stock validation.
/// </summary>
public class CartAppService : ECommerceAppService, ICartAppService
{
    private readonly IRepository<Cart, Guid> _cartRepository;
    private readonly IRepository<CartItem, Guid> _cartItemRepository;
    private readonly IRepository<Catalog.ProductVariant, Guid> _variantRepository;
    private readonly IRepository<Catalog.Product, Guid> _productRepository;
    private readonly IRepository<Catalog.Inventory, Guid> _inventoryRepository;
    private readonly Catalog.IInventoryValidationAppService _inventoryValidation;
    private readonly Catalog.IInventoryReservationService _inventoryReservation;

    public CartAppService(
        IRepository<Cart, Guid> cartRepository,
        IRepository<CartItem, Guid> cartItemRepository,
        IRepository<Catalog.ProductVariant, Guid> variantRepository,
        IRepository<Catalog.Product, Guid> productRepository,
        IRepository<Catalog.Inventory, Guid> inventoryRepository,
        Catalog.IInventoryValidationAppService inventoryValidation,
        Catalog.IInventoryReservationService inventoryReservation)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _variantRepository = variantRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _inventoryValidation = inventoryValidation;
        _inventoryReservation = inventoryReservation;
    }

    /// <inheritdoc />
    [AllowAnonymous]
    public async Task<CartDto> GetCartAsync(Guid? guestCartId = null)
    {
        var cart = await ResolveOrCreateCartAsync(guestCartId);
        return await BuildCartDtoAsync(cart);
    }

    /// <inheritdoc />
    [AllowAnonymous]
    public async Task<CartDto> AddItemAsync(AddCartItemDto input, Guid? guestCartId = null)
    {
        await _inventoryValidation.ValidateVariantAvailabilityAsync(input.ProductVariantId, input.Quantity);

        var cart = await ResolveOrCreateCartAsync(guestCartId);
        var existing = await _cartItemRepository.FirstOrDefaultAsync(x =>
            x.CartId == cart.Id && x.ProductVariantId == input.ProductVariantId);

        if (existing != null)
        {
            var newQty = existing.Quantity + input.Quantity;
            await _inventoryValidation.ValidateVariantAvailabilityAsync(input.ProductVariantId, newQty);
            existing.SetQuantity(newQty);
            await _cartItemRepository.UpdateAsync(existing);
        }
        else
        {
            var item = new CartItem(GuidGenerator.Create(), cart.Id, input.ProductVariantId, input.Quantity);
            await _cartItemRepository.InsertAsync(item);
        }

        await _inventoryReservation.ReserveAsync(input.ProductVariantId, input.Quantity);
        return await BuildCartDtoAsync(cart);
    }

    /// <inheritdoc />
    [AllowAnonymous]
    public async Task<CartDto> UpdateItemAsync(Guid cartItemId, UpdateCartItemDto input, Guid? guestCartId = null)
    {
        var cart = await ResolveOrCreateCartAsync(guestCartId);
        var item = await _cartItemRepository.FirstOrDefaultAsync(x => x.Id == cartItemId && x.CartId == cart.Id);
        if (item == null)
            throw new Volo.Abp.BusinessException("ECommerce:CartItemNotFound").WithData("CartItemId", cartItemId);

        await _inventoryValidation.ValidateVariantAvailabilityAsync(item.ProductVariantId, input.Quantity);
        var oldQty = item.Quantity;
        item.SetQuantity(input.Quantity);
        await _cartItemRepository.UpdateAsync(item);
        await _inventoryReservation.ReleaseAsync(item.ProductVariantId, oldQty);
        await _inventoryReservation.ReserveAsync(item.ProductVariantId, input.Quantity);
        return await BuildCartDtoAsync(cart);
    }

    /// <inheritdoc />
    [AllowAnonymous]
    public async Task<CartDto> RemoveItemAsync(Guid cartItemId, Guid? guestCartId = null)
    {
        var cart = await ResolveOrCreateCartAsync(guestCartId);
        var item = await _cartItemRepository.FirstOrDefaultAsync(x => x.Id == cartItemId && x.CartId == cart.Id);
        if (item != null)
        {
            await _inventoryReservation.ReleaseAsync(item.ProductVariantId, item.Quantity);
            await _cartItemRepository.DeleteAsync(item);
        }
        return await BuildCartDtoAsync(cart);
    }

    /// <inheritdoc />
    [Authorize]
    public async Task<CartDto> MergeGuestCartAsync(Guid guestCartId)
    {
        var userId = CurrentUser.Id ?? throw new Volo.Abp.Authorization.AbpAuthorizationException("User must be logged in to merge cart.");
        var guestCart = await _cartRepository.FirstOrDefaultAsync(c => c.AnonymousId == guestCartId);
        if (guestCart == null)
            return await GetCartAsync(null);

        var userCart = await _cartRepository.FirstOrDefaultAsync(c => c.UserId == userId)
            ?? await CreateCartForUserAsync(userId);

        var guestItems = await _cartItemRepository.GetListAsync(x => x.CartId == guestCart.Id);
        foreach (var gi in guestItems)
        {
            var existingUser = await _cartItemRepository.FirstOrDefaultAsync(x => x.CartId == userCart.Id && x.ProductVariantId == gi.ProductVariantId);
            var newQty = (existingUser?.Quantity ?? 0) + gi.Quantity;
            await _inventoryValidation.ValidateVariantAvailabilityAsync(gi.ProductVariantId, newQty);
        }

        foreach (var gi in guestItems)
            await _inventoryReservation.ReleaseAsync(gi.ProductVariantId, gi.Quantity);

        foreach (var gi in guestItems)
        {
            var existing = await _cartItemRepository.FirstOrDefaultAsync(x =>
                x.CartId == userCart.Id && x.ProductVariantId == gi.ProductVariantId);
            if (existing != null)
            {
                var newQty = existing.Quantity + gi.Quantity;
                existing.SetQuantity(newQty);
                await _cartItemRepository.UpdateAsync(existing);
            }
            else
            {
                var newItem = new CartItem(GuidGenerator.Create(), userCart.Id, gi.ProductVariantId, gi.Quantity);
                await _cartItemRepository.InsertAsync(newItem);
            }
        }

        await _cartItemRepository.DeleteManyAsync(guestItems);
        await _cartRepository.DeleteAsync(guestCart);

        foreach (var gi in guestItems)
            await _inventoryReservation.ReserveAsync(gi.ProductVariantId, gi.Quantity);

        return await BuildCartDtoAsync(userCart);
    }

    private async Task<Cart> ResolveOrCreateCartAsync(Guid? guestCartId)
    {
        var userId = CurrentUser.Id;
        if (userId != null)
        {
            var userCart = await _cartRepository.FirstOrDefaultAsync(c => c.UserId == userId);
            if (userCart != null)
                return userCart;
            return await CreateCartForUserAsync(userId.Value);
        }

        if (guestCartId == null || guestCartId == Guid.Empty)
            throw new Volo.Abp.BusinessException("ECommerce:GuestCartIdRequired")
                .WithData("Message", "Pass guestCartId for guest users (e.g. from cookie or localStorage).");

        var guestCart = await _cartRepository.FirstOrDefaultAsync(c => c.AnonymousId == guestCartId);
        if (guestCart != null)
            return guestCart;
        return await CreateCartForGuestAsync(guestCartId.Value);
    }

    private async Task<Cart> CreateCartForUserAsync(Guid userId)
    {
        var cart = new Cart(GuidGenerator.Create(), userId, null);
        await _cartRepository.InsertAsync(cart);
        return cart;
    }

    private async Task<Cart> CreateCartForGuestAsync(Guid anonymousId)
    {
        var cart = new Cart(GuidGenerator.Create(), null, anonymousId);
        await _cartRepository.InsertAsync(cart);
        return cart;
    }

    private async Task<CartDto> BuildCartDtoAsync(Cart cart)
    {
        var items = await _cartItemRepository.GetListAsync(x => x.CartId == cart.Id);
        if (items.Count == 0)
            return new CartDto
            {
                Id = cart.Id,
                IsAuthenticated = cart.UserId != null,
                ItemCount = 0
            };

        var variantIds = items.Select(x => x.ProductVariantId).Distinct().ToList();
        var variants = await _variantRepository.GetListAsync(v => variantIds.Contains(v.Id));
        var productIds = variants.Select(v => v.ProductId).Distinct().ToList();
        var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));
        var inventories = await _inventoryRepository.GetListAsync(i => variantIds.Contains(i.ProductVariantId));

        var productMap = products.ToDictionary(p => p.Id);
        var variantMap = variants.ToDictionary(v => v.Id);
        var invMap = inventories.ToDictionary(i => i.ProductVariantId);

        var itemDtos = items.Select(item =>
        {
            variantMap.TryGetValue(item.ProductVariantId, out var variant);
            var product = variant != null && productMap.TryGetValue(variant.ProductId, out var p) ? p : null;
            invMap.TryGetValue(item.ProductVariantId, out var inv);
            return new CartItemDto
            {
                Id = item.Id,
                CartId = cart.Id,
                ProductVariantId = item.ProductVariantId,
                ProductId = variant?.ProductId ?? Guid.Empty,
                ProductName = product?.Name ?? "",
                Sku = variant?.Sku ?? "",
                UnitPrice = variant?.Price,
                Quantity = item.Quantity,
                AvailableStock = inv?.AvailableQuantity
            };
        }).ToList();

        return new CartDto
        {
            Id = cart.Id,
            IsAuthenticated = cart.UserId != null,
            Items = itemDtos,
            ItemCount = itemDtos.Sum(x => x.Quantity)
        };
    }
}
