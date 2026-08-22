using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Marketplace;

/// <summary>
/// Port for publishing or updating product listings on an external marketplace.
/// Implementations live in channel-specific adapter projects; core catalog depends only on this contract.
/// </summary>
public interface IListingPublisher
{
    /// <summary>Stable channel key (e.g. "Amazon", "Noon") for configuration and worker dispatch.</summary>
    string ChannelKey { get; }

    /// <summary>
    /// Creates or updates a remote listing from a channel-agnostic snapshot. Mapping to the channel schema is the adapter's responsibility.
    /// </summary>
    Task<PublishListingResult> PublishOrUpdateAsync(
        PublishListingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Removes or deactivates a listing on the channel when the product is withdrawn.</summary>
    Task<WithdrawListingResult> WithdrawAsync(
        WithdrawListingRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>Input for listing publish; adapters map to channel APIs.</summary>
public class PublishListingRequest
{
    public Guid TenantId { get; set; }
    /// <summary>Tenant product id (core catalog).</summary>
    public Guid ProductId { get; set; }
    /// <summary>Optional variant id when the listing is variant-scoped.</summary>
    public Guid? ProductVariantId { get; set; }
    public MarketplaceListingSnapshot Snapshot { get; set; } = new();
    /// <summary>Idempotency key for safe retries (tenant-generated or adapter-generated).</summary>
    public string? IdempotencyKey { get; set; }
}

public class WithdrawListingRequest
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    /// <summary>Remote listing id from a prior publish, if known.</summary>
    public string? RemoteListingId { get; set; }
}

public class PublishListingResult
{
    public bool Success { get; set; }
    public string? RemoteListingId { get; set; }
    public string? RemoteListingUrl { get; set; }
    public string? ErrorMessage { get; set; }
}

public class WithdrawListingResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>Channel-agnostic listing payload; adapters translate to marketplace-specific fields.</summary>
public class MarketplaceListingSnapshot
{
    public string Sku { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? CurrencyCode { get; set; }
    public IReadOnlyDictionary<string, string?> Attributes { get; set; } = new Dictionary<string, string?>();
    public IReadOnlyList<string> ImageUrls { get; set; } = Array.Empty<string>();
}
