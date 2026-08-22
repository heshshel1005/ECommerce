using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ECommerce.Organizations;

/// <summary>
/// Per-tenant organization storefront profile (created after host approval).
/// </summary>
public class OrganizationProfile : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public string DisplayName { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public OrganizationBusinessType BusinessType { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public string? ShortDescription { get; set; }
    public string? LogoFilePathOrKey { get; set; }

    protected OrganizationProfile()
    {
    }

    public OrganizationProfile(
        Guid id,
        Guid tenantId,
        string displayName,
        OrganizationBusinessType businessType,
        string? legalName = null,
        string? website = null,
        string? phone = null,
        string? shortDescription = null,
        string? logoFilePathOrKey = null)
        : base(id)
    {
        TenantId = tenantId;
        DisplayName = displayName ?? string.Empty;
        LegalName = legalName;
        BusinessType = businessType;
        Website = website;
        Phone = phone;
        ShortDescription = shortDescription;
        LogoFilePathOrKey = logoFilePathOrKey;
    }
}
