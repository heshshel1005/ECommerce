using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Marketplace;

/// <summary>
/// Port for ingesting orders that originated on an external marketplace into the tenant's order pipeline.
/// </summary>
public interface IOrderIngress
{
    string ChannelKey { get; }

    /// <summary>
    /// Validates and records an external order (e.g. after webhook or polling). Core domain creates internal order lines as needed.
    /// </summary>
    Task<OrderIngressResult> IngestExternalOrderAsync(
        ExternalMarketplaceOrder envelope,
        CancellationToken cancellationToken = default);
}

/// <summary>Channel-agnostic order envelope; raw channel payload may be attached for auditing.</summary>
public class ExternalMarketplaceOrder
{
    public Guid TenantId { get; set; }
    public string ExternalOrderId { get; set; } = string.Empty;
    public string? ExternalBuyerId { get; set; }
    public IReadOnlyList<ExternalMarketplaceOrderLine> Lines { get; set; } = Array.Empty<ExternalMarketplaceOrderLine>();
    public string? CurrencyCode { get; set; }
    public decimal? TotalAmount { get; set; }
    /// <summary>Optional opaque blob (JSON) for adapter-specific fields not modeled here.</summary>
    public string? RawPayloadJson { get; set; }
}

public class ExternalMarketplaceOrderLine
{
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
}

public class OrderIngressResult
{
    public bool Success { get; set; }
    /// <summary>Internal order id when creation succeeded.</summary>
    public Guid? InternalOrderId { get; set; }
    public string? ErrorMessage { get; set; }
}
