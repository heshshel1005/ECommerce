using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Marketing;

[Volo.Abp.RemoteService(IsEnabled = false)]
public interface IWishlistAppService : IApplicationService
{
    Task<WishlistDto> GetListAsync();
    Task<WishlistDto> AddItemAsync(Guid productVariantId);
    Task<WishlistDto> RemoveItemAsync(Guid wishlistItemId);
    /// <summary>Adds the variant from the wishlist item to the cart and returns the updated cart (does not remove from wishlist).</summary>
    Task AddToCartAsync(Guid wishlistItemId);
}
