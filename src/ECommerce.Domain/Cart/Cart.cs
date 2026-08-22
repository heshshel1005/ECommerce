using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ECommerce.Cart;

/// <summary>
/// Shopping cart. Either linked to a user (UserId) or to a guest (AnonymousId).
/// One cart per user or per anonymous id; persistent for logged-in users.
/// </summary>
public class Cart : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    /// <summary>Set when the cart belongs to an authenticated user.</summary>
    public Guid? UserId { get; set; }

    /// <summary>Set when the cart belongs to a guest (stored in cookie/localStorage).</summary>
    public Guid? AnonymousId { get; set; }

    protected Cart()
    {
    }

    public Cart(Guid id, Guid? userId, Guid? anonymousId)
        : base(id)
    {
        if (userId == null && anonymousId == null)
            throw new ArgumentException("Either UserId or AnonymousId must be set.", nameof(anonymousId));
        if (userId != null && anonymousId != null)
            throw new ArgumentException("Cart cannot have both UserId and AnonymousId.", nameof(anonymousId));

        UserId = userId;
        AnonymousId = anonymousId;
    }
}
