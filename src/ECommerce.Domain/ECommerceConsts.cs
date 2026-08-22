using Volo.Abp.Identity;

namespace ECommerce;

public static class ECommerceConsts
{
    public const string DbTablePrefix = "App";
    public const string? DbSchema = null;
    public const string AdminEmailDefaultValue = IdentityDataSeedContributor.AdminEmailDefaultValue;
    public const string AdminPasswordDefaultValue = "1q2w3E*";

    public static class CustomerProfile
    {
        public const int MaxDisplayNameLength = 256;
        public const int MaxPhoneNumberLength = 32;
    }

    public static class CustomerAddress
    {
        public const int MaxLabelLength = 64;
        public const int MaxStreetLength = 512;
        public const int MaxCityLength = 128;
        public const int MaxRegionLength = 128;
        public const int MaxPostalCodeLength = 32;
        public const int MaxCountryLength = 128;
    }

    public static class Order
    {
        public const int MaxContactEmailLength = 256;
        public const int MaxContactPhoneLength = 32;
        public const int MaxContactNameLength = 256;
        public const int MaxStreetLength = 512;
        public const int MaxCityLength = 128;
        public const int MaxRegionLength = 128;
        public const int MaxPostalCodeLength = 32;
        public const int MaxCountryLength = 128;
        public const int MaxShippingInstructionsLength = 500;
        public const int MaxShippingMethodCodeLength = 64;
        public const int MaxShippingMethodNameLength = 128;
        public const int MaxProductNameLength = 512;
        public const int MaxSkuLength = 64;
        public const int MaxPaymentGatewayLength = 64;
        public const int MaxExternalPaymentIdLength = 256;
        public const int MaxShipmentCarrierLength = 128;
        public const int MaxShipmentTrackingLength = 256;
        public const int MaxShipmentNotesLength = 500;
    }

    public static class Marketing
    {
        public const int CouponMaxCodeLength = 64;
        public const int RegistryMaxTitleLength = 256;
        public const int RegistryMaxSlugLength = 256;
        public const int RegistryItemMaxNoteLength = 500;
        public const int ClaimMaxMessageLength = 500;
        public const int RedemptionRuleMaxNameLength = 256;
        public const int NewsletterSubscriberMaxEmailLength = 256;
        public const int NewsletterSubscriberMaxNameLength = 256;
        public const int PointsTransactionMaxDescriptionLength = 512;
    }

    public static class Catalog
    {
        public const int TranslationLanguageMaxLength = 10;
        public const int CategoryMaxNameLength = 256;
        public const int CategoryMaxSlugLength = 256;
        public const int ProductTypeMaxCodeLength = 64;
        public const int ProductTypeMaxNameLength = 256;
        public const int ProductTypeRuleMaxConditionAttributeKeyLength = 64;
        public const int ProductTypeRuleMaxConditionExpectedValueLength = 256;
        public const int AttributeDefinitionTranslationDisplayNameMaxLength = 256;
        public const int AttributeDefinitionTranslationDescriptionMaxLength = 2000;
        public const int AttributeOptionTranslationDisplayNameMaxLength = 256;
        public const int AttributeOptionValueMaxLength = 256;
        public const int ProductMaxProductNumberLength = 64;
        public const int ProductMaxNameLength = 512;
        public const int ProductMaxDescriptionLength = 4000;
        public const int ProductAttributeMaxNameLength = 128;
        public const int ProductVariantMaxSkuLength = 64;
        public const int ProductVariantAttributeMaxValueLength = 128;
        public const int ProductMediaMaxFilePathLength = 1024;
        public const int ProductMediaMaxAltTextLength = 256;
        public const int ProductReviewMaxReviewTextLength = 2000;
        public const int BrandMaxNameLength = 256;
        public const int BrandMaxSlugLength = 128;
        public const int BrandMaxDescriptionLength = 2000;
        public const int BrandModelMaxNameLength = 256;
        public const int BrandModelMaxCodeLength = 64;
    }

    public static class OrganizationSignup
    {
        public const int MaxTenantNameLength = 64;
        public const int MaxDisplayNameLength = 256;
        public const int MaxLegalNameLength = 256;
        public const int MaxWebsiteLength = 512;
        public const int MaxPhoneLength = 32;
        public const int MaxShortDescriptionLength = 2000;
        public const int MaxLogoPathLength = 1024;
        public const int MaxAdminEmailLength = 256;
        public const int MaxAdminUserNameLength = 256;
        public const int MaxAdminDisplayNameLength = 256;
        public const int MaxAdminPasswordCipherLength = 4096;
        public const int MaxRejectionReasonLength = 2000;
    }

    public static class OrganizationProfile
    {
        public const int MaxDisplayNameLength = 256;
        public const int MaxLegalNameLength = 256;
        public const int MaxWebsiteLength = 512;
        public const int MaxPhoneLength = 32;
        public const int MaxShortDescriptionLength = 2000;
        public const int MaxLogoPathLength = 1024;
    }

    public static class Notification
    {
        public const int MaxTitleLength = 500;
        public const int MaxMessageLength = 2000;
        public const int MaxNotificationTypeLength = 100;
        public const int MaxLinkUrlLength = 500;
        public const int MaxDataLength = 2000;
    }
}
