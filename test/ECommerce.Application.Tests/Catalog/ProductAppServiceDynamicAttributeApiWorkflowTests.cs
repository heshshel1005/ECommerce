using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace ECommerce.Catalog;

public abstract class ProductAppServiceDynamicAttributeApiWorkflowTests<TStartupModule> : ECommerceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IProductAppService _productAppService;
    private readonly IBrandAppService _brandAppService;
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IRepository<ProductType, Guid> _productTypeRepository;
    private readonly IRepository<AttributeDefinition, Guid> _attributeDefinitionRepository;
    private readonly IRepository<AttributeDefinitionTranslation, Guid> _attributeDefinitionTranslationRepository;
    private readonly IRepository<AttributeOption, Guid> _attributeOptionRepository;
    private readonly IRepository<AttributeOptionTranslation, Guid> _attributeOptionTranslationRepository;
    private readonly IRepository<ProductTypeAttributeRule, Guid> _productTypeAttributeRuleRepository;

    protected ProductAppServiceDynamicAttributeApiWorkflowTests()
    {
        _productAppService = GetRequiredService<IProductAppService>();
        _brandAppService = GetRequiredService<IBrandAppService>();
        _productRepository = GetRequiredService<IRepository<Product, Guid>>();
        _productTypeRepository = GetRequiredService<IRepository<ProductType, Guid>>();
        _attributeDefinitionRepository = GetRequiredService<IRepository<AttributeDefinition, Guid>>();
        _attributeDefinitionTranslationRepository = GetRequiredService<IRepository<AttributeDefinitionTranslation, Guid>>();
        _attributeOptionRepository = GetRequiredService<IRepository<AttributeOption, Guid>>();
        _attributeOptionTranslationRepository = GetRequiredService<IRepository<AttributeOptionTranslation, Guid>>();
        _productTypeAttributeRuleRepository = GetRequiredService<IRepository<ProductTypeAttributeRule, Guid>>();
    }

    [Fact]
    public async Task Should_Create_And_Update_Product_With_Dynamic_Attributes_And_Return_Field_Level_Errors()
    {
        var fixture = await CreateValidationFixtureAsync();
        ProductDto? created = null;

        try
        {
            created = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
            {
                ProductNumber = $"API-WF-CREATE-{Guid.NewGuid():N}",
                Name = "API Workflow Product",
                BrandId = fixture.BrandId,
                ProductTypeId = fixture.ProductTypeId,
                DynamicAttributes = new Dictionary<string, object?>
                {
                    ["part_number"] = "PN-200",
                    ["condition"] = "new",
                    ["gtin_upc"] = "123456789012",
                    ["weight_kg"] = 2.5m
                },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "API Workflow Product" }
                }
            }));

            var persistedCreated = await WithDefaultTestTenantAsync(() => _productRepository.GetAsync(created.Id));
            persistedCreated.ProductTypeId.ShouldBe(fixture.ProductTypeId);
            persistedCreated.DynamicAttributesJson.ShouldNotBeNullOrWhiteSpace();
            persistedCreated.DynamicAttributesJson.ShouldContain("\"part_number\":\"PN-200\"");

            await WithDefaultTestTenantAsync(() => _productAppService.UpdateAsync(created.Id, new UpdateProductDto
            {
                ProductNumber = created.ProductNumber,
                Name = "API Workflow Product Updated",
                BrandId = fixture.BrandId,
                ProductTypeId = fixture.ProductTypeId,
                DynamicAttributes = new Dictionary<string, object?>
                {
                    ["part_number"] = "PN-201",
                    ["condition"] = "used",
                    ["gtin_upc"] = "123456789013",
                    ["weight_kg"] = 3.75m
                },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "API Workflow Product Updated" }
                }
            }));

            var persistedUpdated = await WithDefaultTestTenantAsync(() => _productRepository.GetAsync(created.Id));
            persistedUpdated.DynamicAttributesJson.ShouldNotBeNullOrWhiteSpace();
            var json = JsonDocument.Parse(persistedUpdated.DynamicAttributesJson!);
            json.RootElement.GetProperty("part_number").GetString().ShouldBe("PN-201");
            json.RootElement.GetProperty("condition").GetString().ShouldBe("used");

            var missingRequired = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.UpdateAsync(created.Id, new UpdateProductDto
            {
                ProductNumber = created.ProductNumber,
                Name = "API Workflow Product Invalid",
                BrandId = fixture.BrandId,
                ProductTypeId = fixture.ProductTypeId,
                DynamicAttributes = new Dictionary<string, object?>
                {
                    ["condition"] = "new",
                    ["gtin_upc"] = "123456789012"
                },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "API Workflow Product Invalid" }
                }
            })));

            missingRequired.Code.ShouldBe("ECommerce:DynamicAttributeRequired");
            missingRequired.Data["AttributeKey"].ShouldBe("part_number");

            var invalidAllowedValue = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.UpdateAsync(created.Id, new UpdateProductDto
            {
                ProductNumber = created.ProductNumber,
                Name = "API Workflow Product Invalid Enum",
                BrandId = fixture.BrandId,
                ProductTypeId = fixture.ProductTypeId,
                DynamicAttributes = new Dictionary<string, object?>
                {
                    ["part_number"] = "PN-202",
                    ["condition"] = "broken",
                    ["gtin_upc"] = "123456789012"
                },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "API Workflow Product Invalid Enum" }
                }
            })));

            invalidAllowedValue.Code.ShouldBe("ECommerce:DynamicAttributeInvalidAllowedValue");
            invalidAllowedValue.Data["AttributeKey"].ShouldBe("condition");
            invalidAllowedValue.Data["AttributeValue"].ShouldBe("broken");
        }
        finally
        {
            if (created != null)
            {
                await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(created.Id));
            }
        }
    }

    [Fact]
    public async Task Should_Return_Localized_Attribute_Metadata_With_Fallbacks()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        var fixture = await CreateValidationFixtureAsync();

        try
        {
            await SeedDefinitionTranslationsAsync(fixture.PartNumberDefinitionId, fixture.ConditionDefinitionId);

            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var localized = await WithDefaultTestTenantAsync(() => _productAppService.GetAttributeRequirementsByProductTypeAsync(fixture.ProductTypeId));

            localized.RequiredAttributes.Count.ShouldBe(2);
            var partNumber = localized.RequiredAttributes.Find(x => x.Key == "part_number");
            partNumber.ShouldNotBeNull();
            partNumber.DisplayName.ShouldBe("Numero de piece");
            partNumber.DisplayNameLanguage.ShouldBe("fr");
            partNumber.FallbackDisplayName.ShouldBeNull();
            partNumber.FallbackDisplayNameLanguage.ShouldBeNull();
            partNumber.Description.ShouldBe("Identifiant interne de la piece");
            partNumber.FallbackDescription.ShouldBeNull();

            var condition = localized.RequiredAttributes.Find(x => x.Key == "condition");
            condition.ShouldNotBeNull();
            condition.DisplayName.ShouldBe("Condition");
            condition.DisplayNameLanguage.ShouldBe("fr");
            condition.LocalizedOptions.ShouldNotBeNull();
            condition.LocalizedOptions.Count.ShouldBe(3);
            condition.LocalizedOptions.Find(x => x.Value == "new")?.DisplayName.ShouldBe("Neuf");
            condition.LocalizedOptions.Find(x => x.Value == "new")?.FallbackDisplayName.ShouldBeNull();
            condition.LocalizedOptions.Find(x => x.Value == "used")?.DisplayName.ShouldBe("Occasion");
            condition.LocalizedOptions.Find(x => x.Value == "used")?.FallbackDisplayName.ShouldBeNull();
            condition.LocalizedOptions.Find(x => x.Value == "remanufactured")?.DisplayName.ShouldBeNull();
            condition.LocalizedOptions.Find(x => x.Value == "remanufactured")?.FallbackDisplayName.ShouldBe("Remanufactured");

            CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
            CultureInfo.CurrentUICulture = new CultureInfo("tr-TR");
            var fallbackOnly = await WithDefaultTestTenantAsync(() => _productAppService.GetAttributeRequirementsByProductTypeAsync(fixture.ProductTypeId));
            var fallbackPartNumber = fallbackOnly.RequiredAttributes.Find(x => x.Key == "part_number");
            fallbackPartNumber.ShouldNotBeNull();
            fallbackPartNumber.DisplayName.ShouldBeNull();
            fallbackPartNumber.FallbackDisplayName.ShouldBe("Part Number");
            fallbackPartNumber.FallbackDisplayNameLanguage.ShouldBe("en");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    private async Task<ValidationFixture> CreateValidationFixtureAsync()
    {
        var brand = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
        {
            Name = $"Api Workflow Brand {Guid.NewGuid():N}",
            Slug = $"api-workflow-brand-{Guid.NewGuid():N}",
            IsActive = true,
            Translations = new List<BrandTranslationDto>
            {
                new() { Language = "en", Name = "Api Workflow Brand" }
            }
        }));

        var (productTypeId, partNumber, condition) = await WithUnitOfWorkAsync(async () =>
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
            {
                var ptId = Guid.NewGuid();
                var productType = new ProductType(
                    ptId,
                    $"API_WORKFLOW_TYPE_{Guid.NewGuid():N}",
                    "Api Workflow Type");
                productType.SetTranslations(
                    new[]
                    {
                        new ProductTypeTranslation(Guid.NewGuid(), ptId, "en", "Api Workflow Type")
                    },
                    "en");
                await _productTypeRepository.InsertAsync(productType, autoSave: true);

                var pn = await CreateDefinitionAsync("part_number", isRequired: true);
                var cond = await CreateDefinitionAsync("condition", allowedValuesJson: "[\"new\",\"used\",\"remanufactured\"]", isRequired: true);
                var gtin = await CreateDefinitionAsync("gtin_upc", regexPattern: "^\\d{12,14}$", isRecommended: true);
                var weight = await CreateDefinitionAsync("weight_kg", minValue: 0.1m, maxValue: 10m, isRecommended: true);

                await CreateRuleAsync(ptId, pn.Id, 10);
                await CreateRuleAsync(ptId, cond.Id, 20);
                await CreateRuleAsync(ptId, gtin.Id, 30);
                await CreateRuleAsync(ptId, weight.Id, 40);

                return (ptId, pn, cond);
            }
        });

        return new ValidationFixture(productTypeId, brand.Id, partNumber.Id, condition.Id);
    }

    private async Task<AttributeDefinition> CreateDefinitionAsync(
        string key,
        string? allowedValuesJson = null,
        string? regexPattern = null,
        decimal? minValue = null,
        decimal? maxValue = null,
        bool isRequired = false,
        bool isRecommended = false)
    {
        var dataType = string.IsNullOrWhiteSpace(allowedValuesJson)
            ? AttributeDefinitionDataType.Text
            : AttributeDefinitionDataType.Enum;

        var definition = new AttributeDefinition(
            Guid.NewGuid(),
            key,
            dataType,
            allowedValuesJson,
            regexPattern,
            minValue,
            maxValue,
            isRequired,
            isRecommended);
        definition.Publish();

        return await WithDefaultTestTenantAsync(() => _attributeDefinitionRepository.InsertAsync(definition, autoSave: true));
    }

    private async Task CreateRuleAsync(Guid productTypeId, Guid attributeDefinitionId, int order)
    {
        await WithDefaultTestTenantAsync(() => _productTypeAttributeRuleRepository.InsertAsync(
            new ProductTypeAttributeRule(Guid.NewGuid(), productTypeId, attributeDefinitionId, order),
            autoSave: true));
    }

    private async Task SeedDefinitionTranslationsAsync(Guid partNumberDefinitionId, Guid conditionDefinitionId)
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
            {
                await EnsureConditionOptionsAsync(conditionDefinitionId);

                await _attributeDefinitionTranslationRepository.InsertAsync(
                    new AttributeDefinitionTranslation(Guid.NewGuid(), partNumberDefinitionId, "en", "Part Number", "Internal part identifier"),
                    autoSave: true);
                await _attributeDefinitionTranslationRepository.InsertAsync(
                    new AttributeDefinitionTranslation(Guid.NewGuid(), partNumberDefinitionId, "fr", "Numero de piece", "Identifiant interne de la piece"),
                    autoSave: true);
                await _attributeDefinitionTranslationRepository.InsertAsync(
                    new AttributeDefinitionTranslation(Guid.NewGuid(), conditionDefinitionId, "en", "Condition", "Item state"),
                    autoSave: true);
                await _attributeDefinitionTranslationRepository.InsertAsync(
                    new AttributeDefinitionTranslation(Guid.NewGuid(), conditionDefinitionId, "fr", "Condition", "Etat de l'article"),
                    autoSave: true);

                await InsertOptionTranslationAsync(conditionDefinitionId, "new", "en", "New");
                await InsertOptionTranslationAsync(conditionDefinitionId, "new", "fr", "Neuf");
                await InsertOptionTranslationAsync(conditionDefinitionId, "used", "en", "Used");
                await InsertOptionTranslationAsync(conditionDefinitionId, "used", "fr", "Occasion");
                await InsertOptionTranslationAsync(conditionDefinitionId, "remanufactured", "en", "Remanufactured");
            }
        });
    }

    private async Task EnsureConditionOptionsAsync(Guid conditionDefinitionId)
    {
        var values = new[] { "new", "used", "remanufactured" };
        for (var i = 0; i < values.Length; i++)
        {
            var value = values[i];
            var id = AttributeOptionIdFactory.Create(conditionDefinitionId, value);
            if (await _attributeOptionRepository.FindAsync(id) != null)
            {
                continue;
            }

            await _attributeOptionRepository.InsertAsync(new AttributeOption(id, conditionDefinitionId, value, i), autoSave: true);
        }
    }

    private async Task InsertOptionTranslationAsync(Guid definitionId, string optionValue, string language, string displayName)
    {
        var optionId = AttributeOptionIdFactory.Create(definitionId, optionValue);
        await _attributeOptionTranslationRepository.InsertAsync(
            new AttributeOptionTranslation(
                Guid.NewGuid(),
                optionId,
                language,
                displayName),
            autoSave: true);
    }

    private sealed record ValidationFixture(Guid ProductTypeId, Guid BrandId, Guid PartNumberDefinitionId, Guid ConditionDefinitionId);
}
