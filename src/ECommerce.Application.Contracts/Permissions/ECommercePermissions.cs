namespace ECommerce.Permissions;

public static class ECommercePermissions
{
    public const string GroupName = "ECommerce";

    /// <summary>
    /// Parent permission for all admin features. Secures admin UI and APIs.
    /// </summary>
    public const string Administration = GroupName + ".Administration";

    public static class Catalog
    {
        public const string Default = GroupName + ".Catalog";

        /// <summary>Host-only: create/update/delete shared taxonomy rows (<see cref="Volo.Abp.MultiTenancy.IMultiTenant.TenantId"/> null).</summary>
        public const string HostTaxonomy = Default + ".HostTaxonomy";

        public const string Brands = Default + ".Brands";
        public const string BrandModels = Default + ".BrandModels";

        /// <summary>Reject attribute definitions back to draft (reviewer).</summary>
        public const string AttributeDefinitionsReview = Default + ".AttributeDefinitions.Review";

        /// <summary>Publish, archive, or demote attribute definitions (publisher).</summary>
        public const string AttributeDefinitionsPublish = Default + ".AttributeDefinitions.Publish";
    }

    public static class Orders
    {
        public const string Default = GroupName + ".Orders";
    }

    public static class Inventory
    {
        public const string Default = GroupName + ".Inventory";
    }

    public static class Analytics
    {
        public const string Default = GroupName + ".Analytics";
    }

    public static class Cms
    {
        public const string Default = GroupName + ".Cms";
    }

    /// <summary>Host-only: review and approve organization (tenant) signups.</summary>
    public static class TenantSignup
    {
        public const string Manage = GroupName + ".TenantSignup.Manage";
    }
}
