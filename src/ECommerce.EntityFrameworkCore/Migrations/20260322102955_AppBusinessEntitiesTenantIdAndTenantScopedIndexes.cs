using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AppBusinessEntitiesTenantIdAndTenantScopedIndexes : Migration
    {
        /// <summary>
        /// Backfill: assign all existing App* rows to a single default tenant so tenant-scoped unique indexes can be applied.
        /// Picks the earliest-created row in AbpTenants. If the table is empty, TenantId stays null (see NOTICE); create a tenant and run manual UPDATEs before production multi-tenancy.
        /// </summary>
        private const string BackfillAppTablesTenantIdSql = @"
DO $ef$
DECLARE default_tenant uuid;
BEGIN
  SELECT ""Id"" INTO default_tenant FROM ""AbpTenants"" ORDER BY ""CreationTime"" ASC LIMIT 1;
  IF default_tenant IS NULL THEN
    RAISE NOTICE 'AppBusinessEntitiesTenantIdAndTenantScopedIndexes: AbpTenants is empty; App* TenantId columns were not backfilled.';
  ELSE
    UPDATE ""AppBrandModels"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppBrands"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppCartItems"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppCarts"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppCategories"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppCoupons"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppCouponUsages"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppCustomerAddresses"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppCustomerPoints"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppCustomerProfiles"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppGiftRegistries"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppGiftRegistryClaims"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppGiftRegistryItems"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppInventories"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppNewsletterSubscribers"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppOrderLines"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppOrders"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppOrderStatusHistories"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppPointsTransactions"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppProductAttributes"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppProductMedia"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppProductReviews"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppProducts"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppProductVariantAttributes"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppProductVariants"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppRedemptionRules"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppShipments"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppWishlistItems"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
    UPDATE ""AppWishlists"" SET ""TenantId"" = default_tenant WHERE ""TenantId"" IS NULL;
  END IF;
END $ef$;
";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppWishlists_UserId",
                table: "AppWishlists");

            migrationBuilder.DropIndex(
                name: "IX_AppWishlistItems_WishlistId_ProductVariantId",
                table: "AppWishlistItems");

            migrationBuilder.DropIndex(
                name: "IX_AppProductVariants_Sku",
                table: "AppProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_AppProducts_ProductNumber",
                table: "AppProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppProductReviews_ProductId_UserId",
                table: "AppProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_AppProductAttributes_Name",
                table: "AppProductAttributes");

            migrationBuilder.DropIndex(
                name: "IX_AppNewsletterSubscribers_Email",
                table: "AppNewsletterSubscribers");

            migrationBuilder.DropIndex(
                name: "IX_AppInventories_ProductVariantId",
                table: "AppInventories");

            migrationBuilder.DropIndex(
                name: "IX_AppGiftRegistries_Slug",
                table: "AppGiftRegistries");

            migrationBuilder.DropIndex(
                name: "IX_AppCustomerProfiles_UserId",
                table: "AppCustomerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_AppCustomerPoints_UserId",
                table: "AppCustomerPoints");

            migrationBuilder.DropIndex(
                name: "IX_AppCouponUsages_CouponId_UserId",
                table: "AppCouponUsages");

            migrationBuilder.DropIndex(
                name: "IX_AppCoupons_Code",
                table: "AppCoupons");

            migrationBuilder.DropIndex(
                name: "IX_AppCarts_AnonymousId",
                table: "AppCarts");

            migrationBuilder.DropIndex(
                name: "IX_AppCarts_UserId",
                table: "AppCarts");

            migrationBuilder.DropIndex(
                name: "IX_AppCartItems_CartId_ProductVariantId",
                table: "AppCartItems");

            migrationBuilder.DropIndex(
                name: "IX_AppBrandModels_BrandId_Name",
                table: "AppBrandModels");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppWishlists",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppWishlistItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppShipments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppRedemptionRules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppProductVariants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppProductVariantAttributes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppProducts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppProductReviews",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppProductMedia",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppProductAttributes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppPointsTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppOrderStatusHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppOrderLines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppNewsletterSubscribers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppInventories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppGiftRegistryItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppGiftRegistryClaims",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppGiftRegistries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCustomerProfiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCustomerPoints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCustomerAddresses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCouponUsages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCoupons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCarts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppCartItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppBrands",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppBrandModels",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(BackfillAppTablesTenantIdSql);

            migrationBuilder.CreateIndex(
                name: "IX_AppWishlists_TenantId_UserId",
                table: "AppWishlists",
                columns: new[] { "TenantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppWishlistItems_TenantId_WishlistId_ProductVariantId",
                table: "AppWishlistItems",
                columns: new[] { "TenantId", "WishlistId", "ProductVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProductVariants_TenantId_Sku",
                table: "AppProductVariants",
                columns: new[] { "TenantId", "Sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_TenantId_ProductNumber",
                table: "AppProducts",
                columns: new[] { "TenantId", "ProductNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProductReviews_TenantId_ProductId_UserId",
                table: "AppProductReviews",
                columns: new[] { "TenantId", "ProductId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProductAttributes_TenantId_Name",
                table: "AppProductAttributes",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppNewsletterSubscribers_TenantId_Email",
                table: "AppNewsletterSubscribers",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppInventories_TenantId_ProductVariantId",
                table: "AppInventories",
                columns: new[] { "TenantId", "ProductVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppGiftRegistries_TenantId_Slug",
                table: "AppGiftRegistries",
                columns: new[] { "TenantId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCustomerProfiles_TenantId_UserId",
                table: "AppCustomerProfiles",
                columns: new[] { "TenantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCustomerPoints_TenantId_UserId",
                table: "AppCustomerPoints",
                columns: new[] { "TenantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCouponUsages_TenantId_CouponId_UserId",
                table: "AppCouponUsages",
                columns: new[] { "TenantId", "CouponId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCoupons_TenantId_Code",
                table: "AppCoupons",
                columns: new[] { "TenantId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCarts_TenantId_AnonymousId",
                table: "AppCarts",
                columns: new[] { "TenantId", "AnonymousId" },
                unique: true,
                filter: "\"AnonymousId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppCarts_TenantId_UserId",
                table: "AppCarts",
                columns: new[] { "TenantId", "UserId" },
                unique: true,
                filter: "\"UserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppCartItems_TenantId_CartId_ProductVariantId",
                table: "AppCartItems",
                columns: new[] { "TenantId", "CartId", "ProductVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBrandModels_TenantId_BrandId_Name",
                table: "AppBrandModels",
                columns: new[] { "TenantId", "BrandId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppWishlists_TenantId_UserId",
                table: "AppWishlists");

            migrationBuilder.DropIndex(
                name: "IX_AppWishlistItems_TenantId_WishlistId_ProductVariantId",
                table: "AppWishlistItems");

            migrationBuilder.DropIndex(
                name: "IX_AppProductVariants_TenantId_Sku",
                table: "AppProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_AppProducts_TenantId_ProductNumber",
                table: "AppProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppProductReviews_TenantId_ProductId_UserId",
                table: "AppProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_AppProductAttributes_TenantId_Name",
                table: "AppProductAttributes");

            migrationBuilder.DropIndex(
                name: "IX_AppNewsletterSubscribers_TenantId_Email",
                table: "AppNewsletterSubscribers");

            migrationBuilder.DropIndex(
                name: "IX_AppInventories_TenantId_ProductVariantId",
                table: "AppInventories");

            migrationBuilder.DropIndex(
                name: "IX_AppGiftRegistries_TenantId_Slug",
                table: "AppGiftRegistries");

            migrationBuilder.DropIndex(
                name: "IX_AppCustomerProfiles_TenantId_UserId",
                table: "AppCustomerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_AppCustomerPoints_TenantId_UserId",
                table: "AppCustomerPoints");

            migrationBuilder.DropIndex(
                name: "IX_AppCouponUsages_TenantId_CouponId_UserId",
                table: "AppCouponUsages");

            migrationBuilder.DropIndex(
                name: "IX_AppCoupons_TenantId_Code",
                table: "AppCoupons");

            migrationBuilder.DropIndex(
                name: "IX_AppCarts_TenantId_AnonymousId",
                table: "AppCarts");

            migrationBuilder.DropIndex(
                name: "IX_AppCarts_TenantId_UserId",
                table: "AppCarts");

            migrationBuilder.DropIndex(
                name: "IX_AppCartItems_TenantId_CartId_ProductVariantId",
                table: "AppCartItems");

            migrationBuilder.DropIndex(
                name: "IX_AppBrandModels_TenantId_BrandId_Name",
                table: "AppBrandModels");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppWishlists");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppWishlistItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppShipments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppRedemptionRules");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppProductVariants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppProductVariantAttributes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppProductReviews");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppProductMedia");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppProductAttributes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppPointsTransactions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppOrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppOrders");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppOrderLines");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppNewsletterSubscribers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppInventories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppGiftRegistryItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppGiftRegistryClaims");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppGiftRegistries");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCustomerProfiles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCustomerPoints");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCustomerAddresses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCouponUsages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCoupons");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCategories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCarts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppCartItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppBrands");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppBrandModels");

            migrationBuilder.CreateIndex(
                name: "IX_AppWishlists_UserId",
                table: "AppWishlists",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppWishlistItems_WishlistId_ProductVariantId",
                table: "AppWishlistItems",
                columns: new[] { "WishlistId", "ProductVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProductVariants_Sku",
                table: "AppProductVariants",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_ProductNumber",
                table: "AppProducts",
                column: "ProductNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProductReviews_ProductId_UserId",
                table: "AppProductReviews",
                columns: new[] { "ProductId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProductAttributes_Name",
                table: "AppProductAttributes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppNewsletterSubscribers_Email",
                table: "AppNewsletterSubscribers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppInventories_ProductVariantId",
                table: "AppInventories",
                column: "ProductVariantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppGiftRegistries_Slug",
                table: "AppGiftRegistries",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCustomerProfiles_UserId",
                table: "AppCustomerProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCustomerPoints_UserId",
                table: "AppCustomerPoints",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCouponUsages_CouponId_UserId",
                table: "AppCouponUsages",
                columns: new[] { "CouponId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppCoupons_Code",
                table: "AppCoupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCarts_AnonymousId",
                table: "AppCarts",
                column: "AnonymousId",
                unique: true,
                filter: "\"AnonymousId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppCarts_UserId",
                table: "AppCarts",
                column: "UserId",
                unique: true,
                filter: "\"UserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppCartItems_CartId_ProductVariantId",
                table: "AppCartItems",
                columns: new[] { "CartId", "ProductVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBrandModels_BrandId_Name",
                table: "AppBrandModels",
                columns: new[] { "BrandId", "Name" });
        }
    }
}
