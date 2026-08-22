using System;
using System.Collections.Generic;
using ECommerce;
using ECommerce.Catalog;
using ECommerce.Localization;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace ECommerce.Catalog;

public class MultilingualDomainValidationTests
{
    [Theory]
    [MemberData(nameof(DuplicateLanguageCases))]
    public void SetTranslations_Should_Reject_Duplicate_Languages(
        Action setTranslationsAction,
        string expectedErrorCode)
    {
        var exception = Should.Throw<BusinessException>(setTranslationsAction);
        exception.Code.ShouldBe(expectedErrorCode);
    }

    [Theory]
    [MemberData(nameof(MissingDefaultLanguageCases))]
    public void SetTranslations_Should_Require_Default_Language_Translation(
        Action setTranslationsAction,
        string expectedErrorCode)
    {
        var exception = Should.Throw<BusinessException>(setTranslationsAction);
        exception.Code.ShouldBe(expectedErrorCode);
    }

    public static IEnumerable<object[]> DuplicateLanguageCases()
    {
        yield return new object[]
        {
            (Action)(() =>
            {
                var category = new Category(Guid.NewGuid(), "Phones", "phones");
                category.SetTranslations(
                [
                    new CategoryTranslation(Guid.NewGuid(), category.Id, "en", "Phones"),
                    new CategoryTranslation(Guid.NewGuid(), category.Id, " EN ", "Mobiles")
                ],
                "en");
            }),
            ECommerceDomainErrorCodes.CategoryDuplicateTranslationLanguage
        };

        yield return new object[]
        {
            (Action)(() =>
            {
                var brand = new Brand(Guid.NewGuid(), "Acme");
                brand.SetTranslations(
                [
                    new BrandTranslation(Guid.NewGuid(), brand.Id, "en", "Acme"),
                    new BrandTranslation(Guid.NewGuid(), brand.Id, " en ", "Acme Updated")
                ],
                "en");
            }),
            ECommerceDomainErrorCodes.BrandDuplicateTranslationLanguage
        };

        yield return new object[]
        {
            (Action)(() =>
            {
                var brandModel = new BrandModel(Guid.NewGuid(), Guid.NewGuid(), "Model A");
                brandModel.SetTranslations(
                [
                    new BrandModelTranslation(Guid.NewGuid(), brandModel.Id, "en", "Model A"),
                    new BrandModelTranslation(Guid.NewGuid(), brandModel.Id, "EN", "Model A+")
                ],
                "en");
            }),
            ECommerceDomainErrorCodes.BrandModelDuplicateTranslationLanguage
        };

        yield return new object[]
        {
            (Action)(() =>
            {
                var product = new Product(Guid.NewGuid(), "SKU-1", "Laptop", Guid.NewGuid());
                product.SetTranslations(
                [
                    new ProductTranslation(Guid.NewGuid(), product.Id, "en", "Laptop"),
                    new ProductTranslation(Guid.NewGuid(), product.Id, " En ", "Notebook")
                ],
                "en");
            }),
            ECommerceDomainErrorCodes.ProductDuplicateTranslationLanguage
        };

        yield return new object[]
        {
            (Action)(() =>
            {
                var productType = new ProductType(Guid.NewGuid(), "AUTO_PART", "Auto Part");
                productType.SetTranslations(
                [
                    new ProductTypeTranslation(Guid.NewGuid(), productType.Id, "en", "Auto Part"),
                    new ProductTypeTranslation(Guid.NewGuid(), productType.Id, " en ", "Pièce auto")
                ],
                "en");
            }),
            ECommerceDomainErrorCodes.ProductTypeDuplicateTranslationLanguage
        };

        yield return new object[]
        {
            (Action)(() =>
            {
                MultilingualDomainGuard.ValidateRequiredDefaultAndNoDuplicates(
                    new List<TranslationLanguageStub>
                    {
                        new() { Language = "en" },
                        new() { Language = " EN " }
                    },
                    "en",
                    ECommerceDomainErrorCodes.AttributeDefinitionDuplicateTranslationLanguage,
                    ECommerceDomainErrorCodes.AttributeDefinitionDefaultTranslationRequired);
            }),
            ECommerceDomainErrorCodes.AttributeDefinitionDuplicateTranslationLanguage
        };
    }

    public static IEnumerable<object[]> MissingDefaultLanguageCases()
    {
        yield return new object[]
        {
            (Action)(() =>
            {
                var category = new Category(Guid.NewGuid(), "Phones", "phones");
                category.SetTranslations(
                [
                    new CategoryTranslation(Guid.NewGuid(), category.Id, "fr", "Telephones")
                ],
                "en");
            }),
            ECommerceDomainErrorCodes.CategoryDefaultTranslationRequired
        };

        yield return new object[]
        {
            (Action)(() =>
            {
                var brand = new Brand(Guid.NewGuid(), "Acme");
                brand.SetTranslations(
                [
                    new BrandTranslation(Guid.NewGuid(), brand.Id, "de", "Acme DE")
                ],
                "en");
            }),
            ECommerceDomainErrorCodes.BrandDefaultTranslationRequired
        };

        yield return new object[]
        {
            (Action)(() =>
            {
                var brandModel = new BrandModel(Guid.NewGuid(), Guid.NewGuid(), "Model A");
                brandModel.SetTranslations(new List<BrandModelTranslation>(), "en");
            }),
            ECommerceDomainErrorCodes.BrandModelDefaultTranslationRequired
        };

        yield return new object[]
        {
            (Action)(() =>
            {
                var product = new Product(Guid.NewGuid(), "SKU-1", "Laptop", Guid.NewGuid());
                product.SetTranslations(Array.Empty<ProductTranslation>(), "en");
            }),
            ECommerceDomainErrorCodes.ProductDefaultTranslationRequired
        };

        yield return new object[]
        {
            (Action)(() =>
            {
                var productType = new ProductType(Guid.NewGuid(), "APPAREL", "Apparel");
                productType.SetTranslations(
                [
                    new ProductTypeTranslation(Guid.NewGuid(), productType.Id, "fr", "Vêtements")
                ],
                "en");
            }),
            ECommerceDomainErrorCodes.ProductTypeDefaultTranslationRequired
        };

        yield return new object[]
        {
            (Action)(() =>
            {
                MultilingualDomainGuard.ValidateRequiredDefaultAndNoDuplicates(
                    new List<TranslationLanguageStub>
                    {
                        new() { Language = "fr" }
                    },
                    "en",
                    ECommerceDomainErrorCodes.AttributeDefinitionDuplicateTranslationLanguage,
                    ECommerceDomainErrorCodes.AttributeDefinitionDefaultTranslationRequired);
            }),
            ECommerceDomainErrorCodes.AttributeDefinitionDefaultTranslationRequired
        };
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AttributeOption_Should_Reject_Empty_Or_Whitespace_Value(string value)
    {
        var definitionId = Guid.NewGuid();
        var ex = Should.Throw<ArgumentException>(() =>
            new AttributeOption(Guid.NewGuid(), definitionId, value, displayOrder: 0));
        ex.ParamName.ShouldBe("value");
    }

    [Fact]
    public void AttributeOption_Should_Reject_Null_Value()
    {
        var definitionId = Guid.NewGuid();
        var ex = Should.Throw<ArgumentException>(() =>
            new AttributeOption(Guid.NewGuid(), definitionId, null!, displayOrder: 0));
        ex.ParamName.ShouldBe("value");
    }

    [Fact]
    public void AttributeOption_SetValue_Should_Reject_Whitespace()
    {
        var option = new AttributeOption(Guid.NewGuid(), Guid.NewGuid(), "ok", displayOrder: 0);
        var ex = Should.Throw<ArgumentException>(() => option.SetValue(" "));
        ex.ParamName.ShouldBe("value");
    }

    [Fact]
    public void AttributeOption_Should_Reject_Value_Exceeding_Max_Length()
    {
        var tooLong = new string('x', ECommerceConsts.Catalog.AttributeOptionValueMaxLength + 1);
        var ex = Should.Throw<ArgumentException>(() =>
            new AttributeOption(Guid.NewGuid(), Guid.NewGuid(), tooLong, displayOrder: 0));
        ex.ParamName.ShouldBe("value");
    }

    private sealed class TranslationLanguageStub : IEntityTranslation
    {
        public string Language { get; set; } = string.Empty;
    }
}
