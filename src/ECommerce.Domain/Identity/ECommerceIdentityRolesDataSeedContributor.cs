using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace ECommerce.Identity;

/// <summary>
/// Seeds the Customer role and grants E-Commerce admin permissions to the "admin" role.
/// The "admin" role (and default admin user) must be created by <see cref="IdentityDataSeedContributor"/> /
/// <see cref="IIdentityDataSeeder"/> before this contributor runs, or by organization-approval bootstrap.
/// </summary>
public class ECommerceIdentityRolesDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    /// <summary>ABP Identity default admin role name (same as IdentityDataSeedContributor).</summary>
    public const string AdminRoleName = "admin";

    public const string CustomerRoleName = "Customer";

    private const string CatalogPermissionPrefix = "ECommerce.Catalog"; // tenant-only (see Application.Contracts permission definition)

    /// <summary>
    /// Permission names (must match ECommercePermissions in Application.Contracts).
    /// </summary>
    private static readonly string[] AdminPermissionNames =
    {
        "ECommerce.Administration",
        "ECommerce.Catalog",
        "ECommerce.Catalog.Brands",
        "ECommerce.Catalog.BrandModels",
        "ECommerce.Catalog.AttributeDefinitions.Review",
        "ECommerce.Catalog.AttributeDefinitions.Publish",
        "ECommerce.Orders",
        "ECommerce.Inventory",
        "ECommerce.Analytics",
        "ECommerce.Cms",
        // ABP Identity module permissions required to manage tenant users/roles from /identity.
        "AbpIdentity.Users",
        "AbpIdentity.Users.Create",
        "AbpIdentity.Users.Update",
        "AbpIdentity.Users.Delete",
        "AbpIdentity.Users.ManagePermissions",
        "AbpIdentity.Users.Update.ManageRoles",
        "AbpIdentity.Roles",
        "AbpIdentity.Roles.Create",
        "AbpIdentity.Roles.Update",
        "AbpIdentity.Roles.Delete",
        "AbpIdentity.Roles.ManagePermissions"
    };

    /// <summary>Must match <c>ECommercePermissions.TenantSignup.Manage</c> in Application.Contracts. Host admin only.</summary>
    private const string TenantSignupManagePermissionName = "ECommerce.TenantSignup.Manage";

    /// <summary>Must match <c>ECommercePermissions.Catalog.HostTaxonomy</c>. Host admin only.</summary>
    private const string CatalogHostTaxonomyPermissionName = "ECommerce.Catalog.HostTaxonomy";

    private readonly IdentityRoleManager _roleManager;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly IPermissionGrantRepository _permissionGrantRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ECommerceIdentityRolesDataSeedContributor(
        IdentityRoleManager roleManager,
        IIdentityRoleRepository roleRepository,
        IdentityUserManager userManager,
        IPermissionDataSeeder permissionDataSeeder,
        IPermissionGrantRepository permissionGrantRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _roleManager = roleManager;
        _roleRepository = roleRepository;
        _userManager = userManager;
        _permissionDataSeeder = permissionDataSeeder;
        _permissionGrantRepository = permissionGrantRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _unitOfWorkManager = unitOfWorkManager;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await CreateRoleIfNotExistsAsync(CustomerRoleName);
        await GrantAdminPermissionsToAdminRoleAsync(context.TenantId);
        await EnsureCustomerRoleHasNoAdminPermissionsAsync(context.TenantId);
        await EnsureAdminEmailConfirmedAsync();
    }

    private async Task CreateRoleIfNotExistsAsync(string roleName)
    {
        var existingRoles = await _roleRepository.GetListAsync();
        var currentTenantId = _currentTenant.Id;
        var existsInCurrentScope = existingRoles.Any(x => x.Name == roleName && x.TenantId == currentTenantId);
        if (existsInCurrentScope)
        {
            return;
        }

        await _roleManager.CreateAsync(new IdentityRole(_guidGenerator.Create(), roleName, _currentTenant.Id));
    }

    private async Task GrantAdminPermissionsToAdminRoleAsync(Guid? tenantId)
    {
        // "R" = Role permission value provider (Volo.Abp.PermissionManagement.RolePermissionValueProvider.ProviderName)
        var permissionsToGrant = AdminPermissionNames;
        if (tenantId == null)
        {
            // Catalog permissions are Tenant-only (see ECommercePermissionDefinitionProvider), so host-scoped admin
            // must not be granted any Catalog permission.
            permissionsToGrant = AdminPermissionNames
                .Where(p => !p.StartsWith(CatalogPermissionPrefix, StringComparison.Ordinal))
                .ToArray();
        }

        await GrantMissingRolePermissionsAsync(permissionsToGrant, tenantId);

        if (tenantId == null)
        {
            await GrantMissingRolePermissionsAsync(
                new[]
                {
                    TenantSignupManagePermissionName,
                    CatalogHostTaxonomyPermissionName,
                    "ECommerce.Catalog.AttributeDefinitions.Review",
                    "ECommerce.Catalog.AttributeDefinitions.Publish"
                },
                tenantId: null);
        }
    }

    /// <summary>
    /// Seeds only permissions not already granted to the admin role (avoids duplicate-key errors on re-run).
    /// </summary>
    private async Task GrantMissingRolePermissionsAsync(string[] permissionNames, Guid? tenantId)
    {
        var distinct = permissionNames.Distinct(StringComparer.Ordinal).ToArray();
        if (distinct.Length == 0)
        {
            return;
        }

        if (_unitOfWorkManager.Current != null)
        {
            await _unitOfWorkManager.Current.SaveChangesAsync();
        }

        var existing = (await _permissionGrantRepository.GetListAsync("R", AdminRoleName))
            .Select(g => g.Name)
            .ToHashSet(StringComparer.Ordinal);
        var missing = distinct.Where(p => !existing.Contains(p)).ToArray();
        if (missing.Length == 0)
        {
            return;
        }

        await _permissionDataSeeder.SeedAsync("R", AdminRoleName, missing, tenantId);
    }

    /// <summary>
    /// Ensures the Customer role has no admin permissions so Customer users see the same UI as guests (storefront only).
    /// Removes any admin permission grants that may have been assigned to the Customer role by mistake or by another module.
    /// </summary>
    private async Task EnsureCustomerRoleHasNoAdminPermissionsAsync(Guid? tenantId)
    {
        var adminSet = new HashSet<string>(AdminPermissionNames, StringComparer.Ordinal)
        {
            TenantSignupManagePermissionName,
            CatalogHostTaxonomyPermissionName
        };
        var grants = await _permissionGrantRepository.GetListAsync("R", CustomerRoleName);
        foreach (var grant in grants.Where(g => adminSet.Contains(g.Name)))
        {
            await _permissionGrantRepository.DeleteAsync(grant);
        }
    }

    /// <summary>
    /// Ensures the default admin user has EmailConfirmed = true so they can sign in when RequireConfirmedEmail is enabled.
    /// </summary>
    private async Task EnsureAdminEmailConfirmedAsync()
    {
        var admin = await _userManager.FindByEmailAsync(ECommerceConsts.AdminEmailDefaultValue)
            ?? await _userManager.FindByNameAsync("admin");
        if (admin != null && !admin.EmailConfirmed)
        {
            admin.SetEmailConfirmed(true);
            await _userManager.UpdateAsync(admin);
        }
    }
}
