using ECommerce;
using ECommerce.Catalog;
using ECommerce.Identity;
using ECommerce.Organizations;
using ECommerce.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using Volo.Abp.Users;
//using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace ECommerce.OrganizationSignup;

/// <summary>
/// Host-only: list, approve, or reject organization signup requests.
/// </summary>
[Authorize(ECommercePermissions.TenantSignup.Manage)]
public class OrganizationSignupHostAppService : ECommerceAppService, IOrganizationSignupHostAppService
{
    private readonly IRepository<OrganizationSignupRequest, Guid> _signupRepository;
    private readonly IRepository<OrganizationProfile, Guid> _profileRepository;
    private readonly IRepository<Tenant, Guid> _tenantRepository;
    private readonly IIdentityUserRepository _identityUserRepository;
    private readonly TenantManager _tenantManager;
    private readonly IdentityUserManager _userManager;
    private readonly IIdentityDataSeeder _identityDataSeeder;
    private readonly IStringEncryptionService _stringEncryption;
    private readonly IOrganizationSignupLogoStorage _logoStorage;
    private readonly ECommerceIdentityRolesDataSeedContributor _identityRolesDataSeedContributor;
    private readonly CatalogDataSeedContributor _catalogDataSeedContributor;
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _currentTenant;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public OrganizationSignupHostAppService(
        IRepository<OrganizationSignupRequest, Guid> signupRepository,
        IRepository<OrganizationProfile, Guid> profileRepository,
        IIdentityUserRepository identityUserRepository,
        IRepository<Tenant, Guid> tenantRepository,
        TenantManager tenantManager,
        IdentityUserManager userManager,
        IIdentityDataSeeder identityDataSeeder,
        IStringEncryptionService stringEncryption,
        IOrganizationSignupLogoStorage logoStorage,
        ECommerceIdentityRolesDataSeedContributor identityRolesDataSeedContributor,
        CatalogDataSeedContributor catalogDataSeedContributor,
        IConfiguration configuration,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _signupRepository = signupRepository;
        _profileRepository = profileRepository;
        _tenantRepository = tenantRepository;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _identityUserRepository = identityUserRepository;
        _identityDataSeeder = identityDataSeeder;
        _stringEncryption = stringEncryption;
        _logoStorage = logoStorage;
        _identityRolesDataSeedContributor = identityRolesDataSeedContributor;
        _catalogDataSeedContributor = catalogDataSeedContributor;
        _configuration = configuration;
        _currentTenant = currentTenant;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public virtual async Task<PagedResultDto<OrganizationSignupRequestDto>> GetListAsync(OrganizationSignupRequestListRequestDto input)
    {
        EnsureHostSide();

        var query = await _signupRepository.GetQueryableAsync();
        if (input.Status.HasValue)
            query = query.Where(x => x.Status == input.Status.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);
        query = query.OrderByDescending(x => x.CreationTime);
        var take = input.MaxResultCount > 0 ? input.MaxResultCount : 10;
        var items = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(take));

        return new PagedResultDto<OrganizationSignupRequestDto>(
            totalCount,
            items.Select(MapToDto).ToList());
    }

    public virtual async Task<OrganizationSignupRequestDto> GetAsync(Guid id)
    {
        EnsureHostSide();
        var entity = await _signupRepository.GetAsync(id);
        return MapToDto(entity);
    }

    public virtual async Task ApproveAsync(Guid id)
    {
        EnsureHostSide();

        var request = await _signupRepository.GetAsync(id);
        if (request.Status != OrganizationSignupStatus.Pending)
            throw new BusinessException("ECommerce:OrganizationSignupNotPending").WithData("Status", request.Status.ToString());

        if (string.IsNullOrEmpty(request.AdminPasswordCipher))
            throw new BusinessException("ECommerce:OrganizationSignupPasswordMissing");

        string plainPassword;
        try
        {
            plainPassword = _stringEncryption.Decrypt(request.AdminPasswordCipher) ?? string.Empty;
        }
        catch
        {
            throw new BusinessException("ECommerce:OrganizationSignupPasswordDecryptFailed");
        }

        if (string.IsNullOrEmpty(plainPassword))
            throw new BusinessException("ECommerce:OrganizationSignupPasswordDecryptFailed");

        if (await _tenantRepository.AnyAsync(t => t.Name == request.TenantName))
            throw new BusinessException("ECommerce:OrganizationSignupTenantNameTaken").WithData("TenantName", request.TenantName);

        var tenant = await _tenantManager.CreateAsync(request.TenantName);
        var defaultCs = _configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(defaultCs))
            throw new BusinessException("ECommerce:DefaultConnectionStringMissing");

        tenant.SetConnectionString(ConnectionStrings.DefaultConnectionStringName, defaultCs);

        // Persist the tenant and all tenant-scoped bootstrap in one inner UoW. Do not call
        // UpdateAsync/InsertAsync on the outer UoW: the inner UoW's SaveChanges would flush the
        // tenant row first, then the MVC UoW filter would try to save the same Tenant again with a
        // stale concurrency token ("Tenant ... Modified" / 0 rows affected).
        using (var tenantUow = _unitOfWorkManager.Begin(new AbpUnitOfWorkOptions(isTransactional: true), requiresNew: true))
        {
            await _tenantRepository.InsertAsync(tenant);

            using (_currentTenant.Change(tenant.Id))
            {
                // Do not run IDataSeeder here (OpenIddict etc.). Use ABP identity data seeder (user then role),
                // then remove the template user so only the org admin remains.
                var bootstrapEmail = $"_org_signup_{tenant.Id:N}@invalid.local";
                await _identityDataSeeder.SeedAsync(
                    bootstrapEmail,
                    ECommerceConsts.AdminPasswordDefaultValue,
                    tenant.Id,
                    adminUserName: "__tenant_bootstrap");
                var bootstrapUser = await _userManager.FindByEmailAsync(bootstrapEmail);
                if (bootstrapUser != null)
                {
                    (await _userManager.DeleteAsync(bootstrapUser)).CheckErrors();

                    // physical delete
                    await _identityUserRepository.HardDeleteAsync(bootstrapUser);


                }

                await _identityRolesDataSeedContributor.SeedAsync(new DataSeedContext(tenant.Id));
                await _catalogDataSeedContributor.SeedAsync(new DataSeedContext(tenant.Id));

                var profile = new OrganizationProfile(
                    GuidGenerator.Create(),
                    tenant.Id,
                    request.DisplayName,
                    request.BusinessType,
                    request.LegalName,
                    request.Website,
                    request.Phone,
                    request.ShortDescription,
                    request.LogoFilePathOrKey);
                await _profileRepository.InsertAsync(profile);

                var user = new IdentityUser(
                    GuidGenerator.Create(),
                    request.AdminUserName,
                    request.AdminEmail,
                    tenant.Id)
                {
                    Name = request.AdminDisplayName
                };
                user.SetEmailConfirmed(true);

                (await _userManager.CreateAsync(user, plainPassword)).CheckErrors();
                (await _userManager.AddToRoleAsync(user, ECommerceIdentityRolesDataSeedContributor.AdminRoleName)).CheckErrors();
            }

            await tenantUow.CompleteAsync();
        }

        request.Status = OrganizationSignupStatus.Approved;
        request.CreatedTenantId = tenant.Id;
        request.ReviewedTime = Clock.Now;
        request.ReviewerUserId = CurrentUser.Id;
        request.AdminPasswordCipher = null;
        await _signupRepository.UpdateAsync(request);
    }

    public virtual async Task RejectAsync(Guid id, RejectOrganizationSignupDto input)
    {
        EnsureHostSide();

        var request = await _signupRepository.GetAsync(id);
        if (request.Status != OrganizationSignupStatus.Pending)
            throw new BusinessException("ECommerce:OrganizationSignupNotPending").WithData("Status", request.Status.ToString());

        request.Status = OrganizationSignupStatus.Rejected;
        request.RejectionReason = input.Reason.Trim();
        request.ReviewedTime = Clock.Now;
        request.ReviewerUserId = CurrentUser.Id;
        request.AdminPasswordCipher = null;

        if (!string.IsNullOrEmpty(request.LogoFilePathOrKey))
            await _logoStorage.DeleteAsync(request.LogoFilePathOrKey);

        await _signupRepository.UpdateAsync(request);
    }

    public virtual async Task<RepairTenantAdminPermissionsResultDto> RepairTenantAdminPermissionsAsync()
    {
        EnsureHostSide();

        var tenants = await _tenantRepository.GetListAsync();
        var repaired = 0;
        var failed = 0;

        foreach (var tenant in tenants)
        {
            try
            {
                using (var tenantUow = _unitOfWorkManager.Begin(new AbpUnitOfWorkOptions(isTransactional: true), requiresNew: true))
                {
                    using (_currentTenant.Change(tenant.Id))
                    {
                        await _identityRolesDataSeedContributor.SeedAsync(new DataSeedContext(tenant.Id));
                    }

                    await tenantUow.CompleteAsync();
                }

                repaired++;
            }
            catch (Exception ex)
            {
                failed++;
                Logger.LogWarning(ex, "Failed to repair admin permissions for tenant {TenantId} ({TenantName}).", tenant.Id, tenant.Name);
            }
        }

        return new RepairTenantAdminPermissionsResultDto
        {
            TotalTenants = tenants.Count,
            RepairedTenants = repaired,
            FailedTenants = failed
        };
    }

    private void EnsureHostSide()
    {
        if (_currentTenant.Id != null)
            throw new BusinessException("ECommerce:OrganizationSignupHostOnly");
    }

    private static OrganizationSignupRequestDto MapToDto(OrganizationSignupRequest e)
    {
        return new OrganizationSignupRequestDto
        {
            Id = e.Id,
            TenantName = e.TenantName,
            DisplayName = e.DisplayName,
            LegalName = e.LegalName,
            BusinessType = e.BusinessType,
            Website = e.Website,
            Phone = e.Phone,
            ShortDescription = e.ShortDescription,
            LogoFilePathOrKey = e.LogoFilePathOrKey,
            AdminEmail = e.AdminEmail,
            AdminUserName = e.AdminUserName,
            AdminDisplayName = e.AdminDisplayName,
            Status = e.Status,
            RejectionReason = e.RejectionReason,
            ReviewedTime = e.ReviewedTime,
            ReviewerUserId = e.ReviewerUserId,
            CreatedTenantId = e.CreatedTenantId,
            CreationTime = e.CreationTime
        };
    }
}
