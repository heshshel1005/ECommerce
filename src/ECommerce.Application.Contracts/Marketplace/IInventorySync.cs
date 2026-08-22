using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Marketplace;

/// <summary>
/// Port for pushing or pulling stock levels between core catalog and an external channel.
/// </summary>
public interface IInventorySync
{
    string ChannelKey { get; }

    /// <summary>Pushes availability from the tenant catalog to the marketplace.</summary>
    Task<InventoryPushResult> PushAvailabilityAsync(
        InventoryPushRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Optional pull of remote availability (e.g. reserved quantity on the channel). Not all channels support this.
    /// </summary>
    Task<IReadOnlyList<RemoteInventorySnapshot>> PullAvailabilityAsync(
        InventoryPullRequest request,
        CancellationToken cancellationToken = default);
}

public class InventoryPushRequest
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int QuantityAvailable { get; set; }
    public string? RemoteListingId { get; set; }
    public string? IdempotencyKey { get; set; }
}

public class InventoryPushResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class InventoryPullRequest
{
    public Guid TenantId { get; set; }
    public IReadOnlyList<string> RemoteSkuOrListingIds { get; set; } = Array.Empty<string>();
}

public class RemoteInventorySnapshot
{
    public string SkuOrListingId { get; set; } = string.Empty;
    public int? QuantityAvailable { get; set; }
    public int? QuantityReserved { get; set; }
}
