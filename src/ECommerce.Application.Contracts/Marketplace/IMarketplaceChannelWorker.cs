using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Marketplace;

/// <summary>
/// Optional background/sync entry point per channel. Host registers one worker per enabled marketplace adapter.
/// Use for polling, reconciliation, or batched publish queues without coupling catalog domain code to channel SDKs.
/// </summary>
public interface IMarketplaceChannelWorker
{
    string ChannelKey { get; }

    /// <summary>
    /// Runs one sync cycle (e.g. pull orders, push inventory deltas). Implementations should honor cancellation and backoff policies.
    /// </summary>
    Task RunSyncCycleAsync(
        MarketplaceWorkerContext context,
        CancellationToken cancellationToken = default);
}

/// <summary>Execution context for channel workers (tenant scope, feature flags, limits).</summary>
public class MarketplaceWorkerContext
{
    /// <summary>When null, worker may process all tenants configured for this channel (host policy).</summary>
    public Guid? TenantId { get; set; }

    public bool DryRun { get; set; }

    /// <summary>Max entities to process in this cycle; 0 means adapter default.</summary>
    public int MaxBatchSize { get; set; }
}
