using System;
using System.ComponentModel.DataAnnotations;
using ECommerce.Organizations;
using ECommerce;

namespace ECommerce.OrganizationSignup;

public class OrganizationSignupSubmitDto
{
    [Required]
    [StringLength(ECommerceConsts.OrganizationSignup.MaxTenantNameLength)]
    public string TenantName { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.OrganizationSignup.MaxDisplayNameLength)]
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(ECommerceConsts.OrganizationSignup.MaxLegalNameLength)]
    public string? LegalName { get; set; }

    [Required]
    public OrganizationBusinessType BusinessType { get; set; }

    [StringLength(ECommerceConsts.OrganizationSignup.MaxWebsiteLength)]
    public string? Website { get; set; }

    [StringLength(ECommerceConsts.OrganizationSignup.MaxPhoneLength)]
    public string? Phone { get; set; }

    [StringLength(ECommerceConsts.OrganizationSignup.MaxShortDescriptionLength)]
    public string? ShortDescription { get; set; }

    /// <summary>Optional. Must match <see cref="LogoUploadSessionId"/> and path returned from upload.</summary>
    public Guid? LogoUploadSessionId { get; set; }

    [StringLength(ECommerceConsts.OrganizationSignup.MaxLogoPathLength)]
    public string? LogoRelativePath { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(ECommerceConsts.OrganizationSignup.MaxAdminEmailLength)]
    public string AdminEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.OrganizationSignup.MaxAdminUserNameLength)]
    public string AdminUserName { get; set; } = string.Empty;

    [Required]
    [StringLength(ECommerceConsts.OrganizationSignup.MaxAdminDisplayNameLength)]
    public string AdminDisplayName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string AdminPassword { get; set; } = string.Empty;
}

public class OrganizationSignupSubmitResultDto
{
    public Guid RequestId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class OrganizationSignupLogoUploadDto
{
    public Guid UploadSessionId { get; set; }

    [StringLength(ECommerceConsts.OrganizationSignup.MaxLogoPathLength)]
    public string RelativePath { get; set; } = string.Empty;
}
