using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ECommerce.Organizations;

/// <summary>
/// Host-only pending/approved/rejected organization signup. <see cref="TenantId"/> is always null.
/// </summary>
public class OrganizationSignupRequest : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public string TenantName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? LegalName { get; set; }

    public OrganizationBusinessType BusinessType { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public string? ShortDescription { get; set; }
    public string? LogoFilePathOrKey { get; set; }

    public string AdminEmail { get; set; } = string.Empty;
    public string AdminUserName { get; set; } = string.Empty;
    public string AdminDisplayName { get; set; } = string.Empty;
    public string? AdminPasswordCipher { get; set; }

    public OrganizationSignupStatus Status { get; set; } = OrganizationSignupStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime? ReviewedTime { get; set; }
    public Guid? ReviewerUserId { get; set; }
    public Guid? CreatedTenantId { get; set; }

    protected OrganizationSignupRequest()
    {
    }

    public OrganizationSignupRequest(
        Guid id,
        string tenantName,
        string displayName,
        OrganizationBusinessType businessType,
        string adminEmail,
        string adminUserName,
        string adminDisplayName,
        string adminPasswordCipher,
        string? legalName = null,
        string? website = null,
        string? phone = null,
        string? shortDescription = null,
        string? logoFilePathOrKey = null)
        : base(id)
    {
        TenantName = tenantName ?? string.Empty;
        DisplayName = displayName ?? string.Empty;
        LegalName = legalName;
        BusinessType = businessType;
        Website = website;
        Phone = phone;
        ShortDescription = shortDescription;
        LogoFilePathOrKey = logoFilePathOrKey;
        AdminEmail = adminEmail ?? string.Empty;
        AdminUserName = adminUserName ?? string.Empty;
        AdminDisplayName = adminDisplayName ?? string.Empty;
        AdminPasswordCipher = adminPasswordCipher;
    }
}
