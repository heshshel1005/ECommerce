using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Catalog;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Cart;

/// <summary>
/// Releases reserved inventory for carts that have not been updated within a given time window.
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize(ECommerce.Permissions.ECommercePermissions.Administration)]
public class CartReservationExpiryAppService : ECommerceAppService, ICartReservationExpiryService
{
    private readonly IRepository<Cart, Guid> _cartRepository;
    private readonly IRepository<CartItem, Guid> _cartItemRepository;
    private readonly IInventoryReservationService _inventoryReservation;

    public CartReservationExpiryAppService(
        IRepository<Cart, Guid> cartRepository,
        IRepository<CartItem, Guid> cartItemRepository,
        IInventoryReservationService inventoryReservation)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _inventoryReservation = inventoryReservation;
    }

    public async Task<int> ReleaseExpiredReservationsAsync(TimeSpan olderThan)
    {
        var cutoff = DateTime.UtcNow - olderThan;
        var query = await _cartRepository.GetQueryableAsync();
        var staleCartIds = await AsyncExecuter.ToListAsync(
            query.Where(c => c.LastModificationTime < cutoff).Select(c => c.Id));

        if (staleCartIds.Count == 0)
            return 0;

        var items = await _cartItemRepository.GetListAsync(i => staleCartIds.Contains(i.CartId));
        if (items.Count == 0)
            return staleCartIds.Count;

        var toRelease = items.Select(i => (i.ProductVariantId, i.Quantity)).ToList();
        await _inventoryReservation.ReleaseForCartItemsAsync(toRelease);

        return staleCartIds.Count;
    }
}
