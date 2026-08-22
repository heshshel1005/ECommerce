using System;

namespace ECommerce;

public static class ECommerceTestConsts
{
    public const string CollectionDefinitionName = "ECommerce collection";

    /// <summary>
    /// Stable tenant id used for the second data-seed pass in integration tests (multi-tenant catalog and app data).
    /// </summary>
    public static readonly Guid DefaultTenantId = Guid.Parse("55555555-5555-5555-5555-555555555555");
}
