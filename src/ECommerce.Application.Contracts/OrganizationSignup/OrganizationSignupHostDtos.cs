using System;
using System.ComponentModel.DataAnnotations;
using ECommerce;
using ECommerce.Organizations;
using Volo.Abp.Application.Dtos;

namespace ECommerce.OrganizationSignup;

public class OrganizationSignupRequestListRequestDto : PagedAndSortedResultRequestDto
{
    public OrganizationSignupStatus? Status { get; set; }
}

public class OrganizationSignupRequestDto : EntityDto<Guid>
{
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
    public OrganizationSignupStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? ReviewedTime { get; set; }
    public Guid? ReviewerUserId { get; set; }
    public Guid? CreatedTenantId { get; set; }
    public DateTime CreationTime { get; set; }
}

public class RejectOrganizationSignupDto
{
    [Required]
    [StringLength(ECommerceConsts.OrganizationSignup.MaxRejectionReasonLength)]
    public string Reason { get; set; } = string.Empty;
}
