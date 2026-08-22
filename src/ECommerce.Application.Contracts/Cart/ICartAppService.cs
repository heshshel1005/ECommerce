using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Cart;

/// <summary>
/// Cart API: get cart, add/update/remove items, merge guest cart on login.
/// Supports guest (anonymous) and authenticated users; cart is persistent for logged-in users.
/// </summary>
public interface ICartAppService : IApplicationService
{
    /// <summary>
    /// Gets the current cart. For guests pass <paramref name="guestCartId"/> (from cookie/localStorage).
    /// For authenticated users the user's cart is returned; pass <paramref name="guestCartId"/> to merge and return the merged cart.
    /// </summary>
    Task<CartDto> GetCartAsync(Guid? guestCartId = null);

    /// <summary>
    /// Adds a product variant to the cart (or increases quantity if already present). Validates stock.
    /// For guests pass <paramref name="guestCartId"/>; for authenticated users it is optional (used only if no user cart exists yet).
    /// </summary>
    Task<CartDto> AddItemAsync(AddCartItemDto input, Guid? guestCartId = null);

    /// <summary>
    /// Updates the quantity of a cart line. Validates stock.
    /// </summary>
    Task<CartDto> UpdateItemAsync(Guid cartItemId, UpdateCartItemDto input, Guid? guestCartId = null);

    /// <summary>
    /// Removes a line from the cart.
    /// </summary>
    Task<CartDto> RemoveItemAsync(Guid cartItemId, Guid? guestCartId = null);

    /// <summary>
    /// Merges the guest cart into the current user's cart. Call after login. Requires authentication.
    /// </summary>
    Task<CartDto> MergeGuestCartAsync(Guid guestCartId);
}
