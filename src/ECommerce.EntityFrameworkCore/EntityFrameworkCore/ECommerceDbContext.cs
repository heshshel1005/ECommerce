using ECommerce.Catalog;
using ECommerce.Customers;
using ECommerce.Marketing;
using ECommerce.Notifications;
using ECommerce.Organizations;
using ECommerce.Orders;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.CmsKit.Blogs;
using Volo.CmsKit.Comments;
using Volo.CmsKit.EntityFrameworkCore;
using Volo.CmsKit.GlobalResources;
using Volo.CmsKit.MediaDescriptors;
using Volo.CmsKit.Menus;
using Volo.CmsKit.MarkedItems;
using Volo.CmsKit.Pages;
using Volo.CmsKit.Ratings;
using Volo.CmsKit.Reactions;
using Volo.CmsKit.Tags;
using Volo.CmsKit.Users;

namespace ECommerce.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ReplaceDbContext(typeof(ICmsKitDbContext))]
[ConnectionStringName("Default")]
public class ECommerceDbContext :
    AbpDbContext<ECommerceDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext,
    ICmsKitDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    public DbSet<CustomerProfile> CustomerProfiles { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }

    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryTranslation> CategoryTranslations { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<ProductTypeTranslation> ProductTypeTranslations { get; set; }
    public DbSet<AttributeDefinition> AttributeDefinitions { get; set; }
    public DbSet<AttributeDefinitionTranslation> AttributeDefinitionTranslations { get; set; }
    public DbSet<AttributeOption> AttributeOptions { get; set; }
    public DbSet<AttributeOptionTranslation> AttributeOptionTranslations { get; set; }
    public DbSet<ProductTypeAttributeRule> ProductTypeAttributeRules { get; set; }
    public DbSet<ProductAttribute> ProductAttributes { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductTranslation> ProductTranslations { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
    public DbSet<ProductMedia> ProductMedia { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<BrandTranslation> BrandTranslations { get; set; }
    public DbSet<BrandModel> BrandModels { get; set; }
    public DbSet<BrandModelTranslation> BrandModelTranslations { get; set; }
    public DbSet<ECommerce.Cart.Cart> Carts { get; set; }
    public DbSet<ECommerce.Cart.CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<Shipment> Shipments { get; set; }

    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<CouponUsage> CouponUsages { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }
    public DbSet<GiftRegistry> GiftRegistries { get; set; }
    public DbSet<GiftRegistryItem> GiftRegistryItems { get; set; }
    public DbSet<GiftRegistryClaim> GiftRegistryClaims { get; set; }
    public DbSet<CustomerPoints> CustomerPoints { get; set; }
    public DbSet<PointsTransaction> PointsTransactions { get; set; }
    public DbSet<RedemptionRule> RedemptionRules { get; set; }
    public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }
    public DbSet<OrganizationSignupRequest> OrganizationSignupRequests { get; set; }
    public DbSet<OrganizationProfile> OrganizationProfiles { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    // CmsKit
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CmsUser> User { get; set; }
    public DbSet<UserReaction> Reactions { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<EntityTag> EntityTags { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<BlogFeature> BlogFeatures { get; set; }
    public DbSet<MediaDescriptor> MediaDescriptors { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<GlobalResource> GlobalResources { get; set; }
    public DbSet<UserMarkedItem> UserMarkedItems { get; set; }

    #endregion

    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();
        builder.ConfigureCmsKit();

        /* Configure your own tables/entities inside here */

        builder.Entity<CustomerProfile>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "CustomerProfiles", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.DisplayName).HasMaxLength(ECommerceConsts.CustomerProfile.MaxDisplayNameLength);
            b.Property(x => x.PhoneNumber).HasMaxLength(ECommerceConsts.CustomerProfile.MaxPhoneNumberLength);
            b.HasIndex(x => new { x.TenantId, x.UserId }).IsUnique();
        });

        builder.Entity<CustomerAddress>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "CustomerAddresses", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Label).HasMaxLength(ECommerceConsts.CustomerAddress.MaxLabelLength);
            b.Property(x => x.Street).HasMaxLength(ECommerceConsts.CustomerAddress.MaxStreetLength);
            b.Property(x => x.City).HasMaxLength(ECommerceConsts.CustomerAddress.MaxCityLength);
            b.Property(x => x.Region).HasMaxLength(ECommerceConsts.CustomerAddress.MaxRegionLength);
            b.Property(x => x.PostalCode).HasMaxLength(ECommerceConsts.CustomerAddress.MaxPostalCodeLength);
            b.Property(x => x.Country).HasMaxLength(ECommerceConsts.CustomerAddress.MaxCountryLength);
            b.HasIndex(x => x.UserId);
        });

        builder.Entity<Category>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "Categories", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).HasMaxLength(ECommerceConsts.Catalog.CategoryMaxNameLength);
            b.Property(x => x.Slug).HasMaxLength(ECommerceConsts.Catalog.CategoryMaxSlugLength);
            b.HasIndex(x => x.ParentId);
            b.HasIndex(x => new { x.TenantId, x.Slug }).IsUnique();
        });

        builder.Entity<CategoryTranslation>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "CategoryTranslations", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => new { x.CategoryId, x.Language }).IsUnique();
            b.HasOne(x => x.Category)
                .WithMany(x => x.Translations)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductType>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "ProductTypes", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).HasMaxLength(ECommerceConsts.Catalog.ProductTypeMaxCodeLength);
            b.Property(x => x.Name).HasMaxLength(ECommerceConsts.Catalog.ProductTypeMaxNameLength);
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
            b.HasIndex(x => new { x.TenantId, x.Name });
        });

        builder.Entity<ProductTypeTranslation>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "ProductTypeTranslations", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Language).HasMaxLength(ECommerceConsts.Catalog.TranslationLanguageMaxLength);
            b.Property(x => x.Name).HasMaxLength(ECommerceConsts.Catalog.ProductTypeMaxNameLength);
            b.HasIndex(x => new { x.ProductTypeId, x.Language }).IsUnique();
            b.HasOne(x => x.ProductType)
                .WithMany(x => x.Translations)
                .HasForeignKey(x => x.ProductTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AttributeDefinition>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "AttributeDefinitions", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Key).HasMaxLength(ECommerceConsts.Catalog.ProductTypeRuleMaxConditionAttributeKeyLength);
            b.Property(x => x.AllowedValuesJson).HasMaxLength(ECommerceConsts.Catalog.ProductMaxDescriptionLength);
            b.Property(x => x.RegexPattern).HasMaxLength(ECommerceConsts.Catalog.ProductMaxDescriptionLength);
            b.HasIndex(x => new { x.TenantId, x.Key }).IsUnique();
        });

        builder.Entity<AttributeDefinitionTranslation>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "AttributeDefinitionTranslations", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Language).HasMaxLength(ECommerceConsts.Catalog.TranslationLanguageMaxLength);
            b.Property(x => x.DisplayName).HasMaxLength(ECommerceConsts.Catalog.AttributeDefinitionTranslationDisplayNameMaxLength);
            b.Property(x => x.Description).HasMaxLength(ECommerceConsts.Catalog.AttributeDefinitionTranslationDescriptionMaxLength);
            b.HasIndex(x => new { x.AttributeDefinitionId, x.Language }).IsUnique();
            b.HasOne(x => x.AttributeDefinition)
                .WithMany(x => x.Translations)
                .HasForeignKey(x => x.AttributeDefinitionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AttributeOption>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "AttributeOptions", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Value).HasMaxLength(ECommerceConsts.Catalog.AttributeOptionValueMaxLength);
            b.HasIndex(x => new { x.TenantId, x.AttributeDefinitionId, x.Value }).IsUnique();
            b.HasIndex(x => x.AttributeDefinitionId);
            b.HasOne(x => x.AttributeDefinition)
                .WithMany(x => x.Options)
                .HasForeignKey(x => x.AttributeDefinitionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AttributeOptionTranslation>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "AttributeOptionTranslations", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Language).HasMaxLength(ECommerceConsts.Catalog.TranslationLanguageMaxLength);
            b.Property(x => x.DisplayName).HasMaxLength(ECommerceConsts.Catalog.AttributeOptionTranslationDisplayNameMaxLength);
            b.HasIndex(x => new { x.TenantId, x.AttributeOptionId, x.Language }).IsUnique();
            b.HasOne(x => x.AttributeOption)
                .WithMany(x => x.Translations)
                .HasForeignKey(x => x.AttributeOptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductTypeAttributeRule>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "ProductTypeAttributeRules", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.ConditionalAttributeKey).HasMaxLength(ECommerceConsts.Catalog.ProductTypeRuleMaxConditionAttributeKeyLength);
            b.Property(x => x.ConditionalExpectedValue).HasMaxLength(ECommerceConsts.Catalog.ProductTypeRuleMaxConditionExpectedValueLength);
            b.HasIndex(x => x.ProductTypeId);
            b.HasIndex(x => x.AttributeDefinitionId);
            b.HasIndex(x => new { x.TenantId, x.ProductTypeId, x.AttributeDefinitionId }).IsUnique();
            b.HasOne<ProductType>()
                .WithMany()
                .HasForeignKey(x => x.ProductTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne<AttributeDefinition>()
                .WithMany()
                .HasForeignKey(x => x.AttributeDefinitionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductAttribute>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "ProductAttributes", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).HasMaxLength(ECommerceConsts.Catalog.ProductAttributeMaxNameLength);
            b.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });

        builder.Entity<Brand>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "Brands", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).HasMaxLength(ECommerceConsts.Catalog.BrandMaxNameLength);
            b.Property(x => x.Slug).HasMaxLength(ECommerceConsts.Catalog.BrandMaxSlugLength);
            b.Property(x => x.Description).HasMaxLength(ECommerceConsts.Catalog.BrandMaxDescriptionLength);
            b.HasIndex(x => x.Name);
        });

        builder.Entity<BrandTranslation>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "BrandTranslations", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => new { x.BrandId, x.Language }).IsUnique();
            b.HasOne(x => x.Brand)
                .WithMany(x => x.Translations)
                .HasForeignKey(x => x.BrandId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<BrandModel>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "BrandModels", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).HasMaxLength(ECommerceConsts.Catalog.BrandModelMaxNameLength);
            b.Property(x => x.Code).HasMaxLength(ECommerceConsts.Catalog.BrandModelMaxCodeLength);
            b.HasIndex(x => x.BrandId);
            b.HasIndex(x => new { x.TenantId, x.BrandId, x.Name });
            b.HasOne(x => x.Brand).WithMany(x => x.Models).HasForeignKey(x => x.BrandId).IsRequired();
        });

        builder.Entity<BrandModelTranslation>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "BrandModelTranslations", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => new { x.BrandModelId, x.Language }).IsUnique();
            b.HasOne(x => x.BrandModel)
                .WithMany(x => x.Translations)
                .HasForeignKey(x => x.BrandModelId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Product>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "Products", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.ProductNumber).HasMaxLength(ECommerceConsts.Catalog.ProductMaxProductNumberLength);
            b.Property(x => x.Name).HasMaxLength(ECommerceConsts.Catalog.ProductMaxNameLength);
            b.Property(x => x.Description).HasMaxLength(ECommerceConsts.Catalog.ProductMaxDescriptionLength);
            b.HasIndex(x => new { x.TenantId, x.ProductNumber }).IsUnique();
            b.HasIndex(x => x.CategoryId);
            b.HasIndex(x => x.BrandId);
            b.HasIndex(x => x.ModelId);
            b.HasOne(x => x.Brand).WithMany().HasForeignKey(x => x.BrandId).IsRequired();
            b.HasOne(x => x.Model).WithMany().HasForeignKey(x => x.ModelId).IsRequired(false);
        });

        builder.Entity<ProductTranslation>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "ProductTranslations", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => new { x.ProductId, x.Language }).IsUnique();
            b.HasOne(x => x.Product)
                .WithMany(x => x.Translations)
                .HasForeignKey(x => x.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductVariant>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "ProductVariants", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Sku).HasMaxLength(ECommerceConsts.Catalog.ProductVariantMaxSkuLength);
            b.Property(x => x.DynamicAttributesJson).HasMaxLength(ECommerceConsts.Catalog.ProductMaxDescriptionLength);
            b.HasIndex(x => new { x.TenantId, x.Sku }).IsUnique();
            b.HasIndex(x => x.ProductId);
        });

        builder.Entity<ProductVariantAttribute>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "ProductVariantAttributes", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Value).HasMaxLength(ECommerceConsts.Catalog.ProductVariantAttributeMaxValueLength);
            b.HasIndex(x => x.ProductVariantId);
            b.HasIndex(x => x.ProductAttributeId);
        });

        builder.Entity<ProductMedia>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "ProductMedia", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.FilePathOrBlobKey).HasMaxLength(ECommerceConsts.Catalog.ProductMediaMaxFilePathLength);
            b.Property(x => x.AltText).HasMaxLength(ECommerceConsts.Catalog.ProductMediaMaxAltTextLength);
            b.HasIndex(x => x.ProductId);
        });

        builder.Entity<Inventory>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "Inventories", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => new { x.TenantId, x.ProductVariantId }).IsUnique();
        });

        builder.Entity<ProductReview>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "ProductReviews", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.ReviewText).HasMaxLength(ECommerceConsts.Catalog.ProductReviewMaxReviewTextLength);
            b.HasIndex(x => x.ProductId);
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => new { x.TenantId, x.ProductId, x.UserId }).IsUnique();
        });

        builder.Entity<ECommerce.Cart.Cart>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "Carts", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => new { x.TenantId, x.UserId }).IsUnique().HasFilter("\"UserId\" IS NOT NULL");
            b.HasIndex(x => new { x.TenantId, x.AnonymousId }).IsUnique().HasFilter("\"AnonymousId\" IS NOT NULL");
        });

        builder.Entity<ECommerce.Cart.CartItem>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "CartItems", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => x.CartId);
            b.HasIndex(x => new { x.TenantId, x.CartId, x.ProductVariantId }).IsUnique();
        });

        builder.Entity<Order>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "Orders", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.ContactEmail).HasMaxLength(ECommerceConsts.Order.MaxContactEmailLength);
            b.Property(x => x.ContactPhone).HasMaxLength(ECommerceConsts.Order.MaxContactPhoneLength);
            b.Property(x => x.ContactName).HasMaxLength(ECommerceConsts.Order.MaxContactNameLength);
            b.Property(x => x.ShippingStreet).HasMaxLength(ECommerceConsts.Order.MaxStreetLength);
            b.Property(x => x.ShippingStreet2).HasMaxLength(ECommerceConsts.Order.MaxStreetLength);
            b.Property(x => x.ShippingCity).HasMaxLength(ECommerceConsts.Order.MaxCityLength);
            b.Property(x => x.ShippingRegion).HasMaxLength(ECommerceConsts.Order.MaxRegionLength);
            b.Property(x => x.ShippingPostalCode).HasMaxLength(ECommerceConsts.Order.MaxPostalCodeLength);
            b.Property(x => x.ShippingCountry).HasMaxLength(ECommerceConsts.Order.MaxCountryLength);
            b.Property(x => x.ShippingInstructions).HasMaxLength(ECommerceConsts.Order.MaxShippingInstructionsLength);
            b.Property(x => x.BillingStreet).HasMaxLength(ECommerceConsts.Order.MaxStreetLength);
            b.Property(x => x.BillingStreet2).HasMaxLength(ECommerceConsts.Order.MaxStreetLength);
            b.Property(x => x.BillingCity).HasMaxLength(ECommerceConsts.Order.MaxCityLength);
            b.Property(x => x.BillingRegion).HasMaxLength(ECommerceConsts.Order.MaxRegionLength);
            b.Property(x => x.BillingPostalCode).HasMaxLength(ECommerceConsts.Order.MaxPostalCodeLength);
            b.Property(x => x.BillingCountry).HasMaxLength(ECommerceConsts.Order.MaxCountryLength);
            b.Property(x => x.ShippingMethodCode).HasMaxLength(ECommerceConsts.Order.MaxShippingMethodCodeLength);
            b.Property(x => x.ShippingMethodName).HasMaxLength(ECommerceConsts.Order.MaxShippingMethodNameLength);
            b.Property(x => x.PaymentStatus);
            b.Property(x => x.PaymentGateway).HasMaxLength(ECommerceConsts.Order.MaxPaymentGatewayLength);
            b.Property(x => x.ExternalPaymentId).HasMaxLength(ECommerceConsts.Order.MaxExternalPaymentIdLength);
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.Status);
            b.HasMany(o => o.Lines).WithOne().HasForeignKey(l => l.OrderId).IsRequired();
        });

        builder.Entity<OrderLine>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "OrderLines", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.ProductName).HasMaxLength(ECommerceConsts.Order.MaxProductNameLength);
            b.Property(x => x.Sku).HasMaxLength(ECommerceConsts.Order.MaxSkuLength);
            b.HasIndex(x => x.OrderId);
        });

        builder.Entity<OrderStatusHistory>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "OrderStatusHistories", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => x.OrderId);
        });

        builder.Entity<Shipment>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "Shipments", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Carrier).HasMaxLength(ECommerceConsts.Order.MaxShipmentCarrierLength);
            b.Property(x => x.TrackingNumber).HasMaxLength(ECommerceConsts.Order.MaxShipmentTrackingLength);
            b.Property(x => x.Notes).HasMaxLength(ECommerceConsts.Order.MaxShipmentNotesLength);
            b.HasIndex(x => x.OrderId);
        });

        builder.Entity<Coupon>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "Coupons", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Code).HasMaxLength(ECommerceConsts.Marketing.CouponMaxCodeLength);
            b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        });

        builder.Entity<CouponUsage>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "CouponUsages", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => x.CouponId);
            b.HasIndex(x => new { x.TenantId, x.CouponId, x.UserId }).IsUnique();
        });

        builder.Entity<Wishlist>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "Wishlists", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => new { x.TenantId, x.UserId }).IsUnique();
        });

        builder.Entity<WishlistItem>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "WishlistItems", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => x.WishlistId);
            b.HasIndex(x => new { x.TenantId, x.WishlistId, x.ProductVariantId }).IsUnique();
        });

        builder.Entity<GiftRegistry>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "GiftRegistries", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Title).HasMaxLength(ECommerceConsts.Marketing.RegistryMaxTitleLength);
            b.Property(x => x.Slug).HasMaxLength(ECommerceConsts.Marketing.RegistryMaxSlugLength);
            b.HasIndex(x => x.OwnerUserId);
            b.HasIndex(x => new { x.TenantId, x.Slug }).IsUnique();
        });

        builder.Entity<GiftRegistryItem>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "GiftRegistryItems", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Note).HasMaxLength(ECommerceConsts.Marketing.RegistryItemMaxNoteLength);
            b.HasIndex(x => x.GiftRegistryId);
        });

        builder.Entity<GiftRegistryClaim>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "GiftRegistryClaims", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Message).HasMaxLength(ECommerceConsts.Marketing.ClaimMaxMessageLength);
            b.HasIndex(x => x.GiftRegistryItemId);
        });

        builder.Entity<CustomerPoints>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "CustomerPoints", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Tier).HasMaxLength(64);
            b.HasIndex(x => new { x.TenantId, x.UserId }).IsUnique();
        });

        builder.Entity<PointsTransaction>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "PointsTransactions", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Description).HasMaxLength(ECommerceConsts.Marketing.PointsTransactionMaxDescriptionLength);
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.OrderId);
        });

        builder.Entity<RedemptionRule>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "RedemptionRules", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).HasMaxLength(ECommerceConsts.Marketing.RedemptionRuleMaxNameLength);
        });

        builder.Entity<NewsletterSubscriber>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "NewsletterSubscribers", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Email).HasMaxLength(ECommerceConsts.Marketing.NewsletterSubscriberMaxEmailLength);
            b.Property(x => x.Name).HasMaxLength(ECommerceConsts.Marketing.NewsletterSubscriberMaxNameLength);
            b.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
        });

        builder.Entity<OrganizationSignupRequest>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "OrganizationSignupRequests", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantName).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxTenantNameLength);
            b.Property(x => x.DisplayName).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxDisplayNameLength);
            b.Property(x => x.LegalName).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxLegalNameLength);
            b.Property(x => x.Website).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxWebsiteLength);
            b.Property(x => x.Phone).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxPhoneLength);
            b.Property(x => x.ShortDescription).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxShortDescriptionLength);
            b.Property(x => x.LogoFilePathOrKey).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxLogoPathLength);
            b.Property(x => x.AdminEmail).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxAdminEmailLength);
            b.Property(x => x.AdminUserName).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxAdminUserNameLength);
            b.Property(x => x.AdminDisplayName).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxAdminDisplayNameLength);
            b.Property(x => x.AdminPasswordCipher).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxAdminPasswordCipherLength);
            b.Property(x => x.RejectionReason).HasMaxLength(ECommerceConsts.OrganizationSignup.MaxRejectionReasonLength);
            b.Property(x => x.BusinessType);
            b.Property(x => x.Status);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CreatedTenantId);
            b.HasIndex(x => x.TenantName)
                .IsUnique()
                .HasFilter("\"Status\" = 0");
        });

        builder.Entity<OrganizationProfile>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "OrganizationProfiles", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.DisplayName).HasMaxLength(ECommerceConsts.OrganizationProfile.MaxDisplayNameLength);
            b.Property(x => x.LegalName).HasMaxLength(ECommerceConsts.OrganizationProfile.MaxLegalNameLength);
            b.Property(x => x.Website).HasMaxLength(ECommerceConsts.OrganizationProfile.MaxWebsiteLength);
            b.Property(x => x.Phone).HasMaxLength(ECommerceConsts.OrganizationProfile.MaxPhoneLength);
            b.Property(x => x.ShortDescription).HasMaxLength(ECommerceConsts.OrganizationProfile.MaxShortDescriptionLength);
            b.Property(x => x.LogoFilePathOrKey).HasMaxLength(ECommerceConsts.OrganizationProfile.MaxLogoPathLength);
            b.Property(x => x.BusinessType);
            b.HasIndex(x => x.TenantId).IsUnique();
        });

        builder.Entity<UserNotification>(b =>
        {
            b.ToTable(ECommerceConsts.DbTablePrefix + "UserNotifications", ECommerceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Title).IsRequired().HasMaxLength(ECommerceConsts.Notification.MaxTitleLength);
            b.Property(x => x.Message).HasMaxLength(ECommerceConsts.Notification.MaxMessageLength);
            b.Property(x => x.NotificationType).HasMaxLength(ECommerceConsts.Notification.MaxNotificationTypeLength);
            b.Property(x => x.LinkUrl).HasMaxLength(ECommerceConsts.Notification.MaxLinkUrlLength);
            b.Property(x => x.Data).HasMaxLength(ECommerceConsts.Notification.MaxDataLength);
            b.HasIndex(x => new { x.TenantId, x.UserId });
            b.HasIndex(x => new { x.TenantId, x.UserId, x.IsRead });
            b.HasIndex(x => new { x.TenantId, x.IsActive });
        });
    }
}
