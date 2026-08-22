using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Marketing;

/// <summary>
/// Public and owner APIs for gift registries.
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
public interface IGiftRegistryAppService : IApplicationService
{
    /// <summary>Get a public registry by slug (for viewing and claiming).</summary>
    Task<GiftRegistryDto?> GetBySlugAsync(string slug);

    /// <summary>Reserve/purchase items on a registry. Optionally add to cart.</summary>
    Task ClaimAsync(ClaimRegistryItemDto input);

    /// <summary>Create a new registry (owner).</summary>
    Task<GiftRegistryDto> CreateAsync(CreateGiftRegistryDto input);

    /// <summary>Get current user's registries.</summary>
    Task<System.Collections.Generic.List<GiftRegistryDto>> GetMyRegistriesAsync();

    /// <summary>Add an item to a registry (owner).</summary>
    Task<GiftRegistryDto> AddItemAsync(Guid giftRegistryId, AddGiftRegistryItemDto input);

    /// <summary>Remove an item from a registry (owner).</summary>
    Task<GiftRegistryDto> RemoveItemAsync(Guid giftRegistryId, Guid giftRegistryItemId);
}
