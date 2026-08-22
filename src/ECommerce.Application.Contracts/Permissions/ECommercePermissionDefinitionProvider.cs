using ECommerce.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace ECommerce.Permissions;

public class ECommercePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var eCommerceGroup = context.AddGroup(ECommercePermissions.GroupName, L("Permission:ECommerce"));

        // Host should have access to admin shell and tenant-agnostic administration features.
        var administration = eCommerceGroup.AddPermission(ECommercePermissions.Administration, L("Permission:Administration"));
        administration.MultiTenancySide = MultiTenancySides.Both;

        // Catalog permissions must be tenant-only.
        var catalog = administration.AddChild(ECommercePermissions.Catalog.Default, L("Permission:Catalog"));
        catalog.MultiTenancySide = MultiTenancySides.Tenant;
        var hostCatalogTaxonomy = administration.AddChild(ECommercePermissions.Catalog.HostTaxonomy, L("Permission:Catalog.HostTaxonomy"));
        hostCatalogTaxonomy.MultiTenancySide = MultiTenancySides.Host;
        var brands = catalog.AddChild(ECommercePermissions.Catalog.Brands, L("Permission:Catalog.Brands"));
        brands.MultiTenancySide = MultiTenancySides.Tenant;
        var brandModels = catalog.AddChild(ECommercePermissions.Catalog.BrandModels, L("Permission:Catalog.BrandModels"));
        brandModels.MultiTenancySide = MultiTenancySides.Tenant;
        var attributeDefinitionsReview = catalog.AddChild(ECommercePermissions.Catalog.AttributeDefinitionsReview, L("Permission:Catalog.AttributeDefinitions.Review"));
        attributeDefinitionsReview.MultiTenancySide = MultiTenancySides.Both;
        var attributeDefinitionsPublish = catalog.AddChild(ECommercePermissions.Catalog.AttributeDefinitionsPublish, L("Permission:Catalog.AttributeDefinitions.Publish"));
        attributeDefinitionsPublish.MultiTenancySide = MultiTenancySides.Both;
        administration.AddChild(ECommercePermissions.Orders.Default, L("Permission:Orders"));
        administration.AddChild(ECommercePermissions.Inventory.Default, L("Permission:Inventory"));
        administration.AddChild(ECommercePermissions.Analytics.Default, L("Permission:Analytics"));
        administration.AddChild(ECommercePermissions.Cms.Default, L("Permission:Cms"));
        var tenantSignupManage = administration.AddChild(ECommercePermissions.TenantSignup.Manage, L("Permission:TenantSignup.Manage"));
        tenantSignupManage.MultiTenancySide = MultiTenancySides.Host;
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ECommerceResource>(name);
    }
}
