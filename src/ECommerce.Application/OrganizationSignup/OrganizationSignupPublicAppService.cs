using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Abp.TenantManagement;

namespace ECommerce.OrganizationSignup;

/// <summary>
/// Anonymous organization signup: logo upload and submit with encrypted admin password (host-scoped rows).
/// </summary>
public class OrganizationSignupPublicAppService : ECommerceAppService, IOrganizationSignupPublicAppService
{
    private const int MaxLogoBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedLogoExtensions = { ".jpg", ".jpeg", ".jpe", ".png", ".gif", ".webp", ".svg" };

    /// <summary>Used when <see cref="IFormFile.FileName"/> has no or an unknown extension (common on mobile: "image", "blob").</summary>
    private static readonly Dictionary<string, string> AllowedLogoMimeToExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/jpg"] = ".jpg",
        ["image/pjpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/x-png"] = ".png",
        ["image/gif"] = ".gif",
        ["image/webp"] = ".webp",
        ["image/svg+xml"] = ".svg"
    };

    private readonly IRepository<OrganizationSignupRequest, Guid> _signupRepository;
    private readonly IRepository<Tenant, Guid> _tenantRepository;
    private readonly IOrganizationSignupLogoStorage _logoStorage;
    private readonly IStringEncryptionService _stringEncryption;
    private readonly ICurrentTenant _currentTenant;

    public OrganizationSignupPublicAppService(
        IRepository<OrganizationSignupRequest, Guid> signupRepository,
        IRepository<Tenant, Guid> tenantRepository,
        IOrganizationSignupLogoStorage logoStorage,
        IStringEncryptionService stringEncryption,
        ICurrentTenant currentTenant)
    {
        _signupRepository = signupRepository;
        _tenantRepository = tenantRepository;
        _logoStorage = logoStorage;
        _stringEncryption = stringEncryption;
        _currentTenant = currentTenant;
    }

    [AllowAnonymous]
    public virtual async Task<OrganizationSignupLogoUploadDto> UploadLogoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new BusinessException("ECommerce:OrganizationSignupLogoRequired");

        var fileName = file.FileName ?? "file";
        var ext = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(ext) || !AllowedLogoExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            var mime = file.ContentType?.Split(';', 2)[0].Trim();
            if (string.IsNullOrEmpty(mime) || !AllowedLogoMimeToExtension.TryGetValue(mime, out var canonicalExt))
                throw new BusinessException("ECommerce:OrganizationSignupLogoInvalidType");

            var baseName = Path.GetFileNameWithoutExtension(fileName);
            if (string.IsNullOrWhiteSpace(baseName))
                baseName = "logo";
            fileName = baseName + canonicalExt;
        }

        if (file.Length > MaxLogoBytes)
            throw new BusinessException("ECommerce:OrganizationSignupLogoTooLarge").WithData("MaxMb", MaxLogoBytes / (1024 * 1024));

        var sessionId = GuidGenerator.Create();

        using (_currentTenant.Change(null))
        {
            await using var stream = file.OpenReadStream();
            var relativePath = await _logoStorage.SaveAsync(stream, fileName, sessionId);
            return new OrganizationSignupLogoUploadDto
            {
                UploadSessionId = sessionId,
                RelativePath = relativePath
            };
        }
    }

    [AllowAnonymous]
    public virtual async Task<OrganizationSignupSubmitResultDto> SubmitAsync(OrganizationSignupSubmitDto input)
    {
        var tenantName = input.TenantName.Trim();
        if (string.IsNullOrEmpty(tenantName))
            throw new BusinessException("ECommerce:OrganizationSignupTenantNameRequired");

        var hasSession = input.LogoUploadSessionId.HasValue;
        var hasPath = !string.IsNullOrWhiteSpace(input.LogoRelativePath);
        if (hasSession != hasPath)
            throw new BusinessException("ECommerce:OrganizationSignupLogoIncomplete");

        string? logoPath = null;
        if (hasSession && input.LogoUploadSessionId.HasValue && !string.IsNullOrWhiteSpace(input.LogoRelativePath))
        {
            ValidateLogoPathBelongsToSession(input.LogoUploadSessionId.Value, input.LogoRelativePath);
            logoPath = input.LogoRelativePath.Trim();
        }

        var cipher = _stringEncryption.Encrypt(input.AdminPassword)
            ?? throw new BusinessException("ECommerce:OrganizationSignupEncryptFailed");
        var id = GuidGenerator.Create();

        using (_currentTenant.Change(null))
        {
            if (await _tenantRepository.AnyAsync(t => t.Name == tenantName))
                throw new BusinessException("ECommerce:OrganizationSignupTenantNameTaken").WithData("TenantName", tenantName);

            if (await _signupRepository.AnyAsync(r =>
                    r.TenantName == tenantName && r.Status == OrganizationSignupStatus.Pending))
                throw new BusinessException("ECommerce:OrganizationSignupTenantNamePending").WithData("TenantName", tenantName);

            var entity = new OrganizationSignupRequest(
                id,
                tenantName,
                input.DisplayName.Trim(),
                input.BusinessType,
                input.AdminEmail.Trim(),
                input.AdminUserName.Trim(),
                input.AdminDisplayName.Trim(),
                cipher,
                string.IsNullOrWhiteSpace(input.LegalName) ? null : input.LegalName.Trim(),
                string.IsNullOrWhiteSpace(input.Website) ? null : input.Website.Trim(),
                string.IsNullOrWhiteSpace(input.Phone) ? null : input.Phone.Trim(),
                string.IsNullOrWhiteSpace(input.ShortDescription) ? null : input.ShortDescription.Trim(),
                logoPath)
            {
                TenantId = null
            };

            await _signupRepository.InsertAsync(entity);
        }

        return new OrganizationSignupSubmitResultDto
        {
            RequestId = id,
            Message = L["OrganizationSignup:SubmitPendingMessage"].Value
        };
    }

    private static void ValidateLogoPathBelongsToSession(Guid sessionId, string relativePath)
    {
        var normalized = relativePath.Replace('\\', '/').TrimStart('/');
        var expectedPrefix = $"App_Data/OrganizationSignupLogos/{sessionId:N}/";
        if (!normalized.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
            throw new BusinessException("ECommerce:OrganizationSignupLogoPathInvalid");
    }
}
