using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace ECommerce.Catalog;

/// <summary>
/// Attribute definition translation resolution and dynamic attribute validation (integration tests).
/// </summary>
public abstract class AttributeDefinitionAppServiceTranslationAndValidationTests<TStartupModule> : ECommerceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IAttributeDefinitionAppService _attributeDefinitionAppService;
    private readonly IProductAppService _productAppService;
    private readonly IBrandAppService _brandAppService;
    private readonly IRepository<AttributeDefinition, Guid> _attributeDefinitionRepository;
    private readonly IRepository<AttributeDefinitionTranslation, Guid> _attributeDefinitionTranslationRepository;
    private readonly IRepository<ProductType, Guid> _productTypeRepository;
    private readonly IRepository<ProductTypeAttributeRule, Guid> _productTypeAttributeRuleRepository;

    protected AttributeDefinitionAppServiceTranslationAndValidationTests()
    {
        _attributeDefinitionAppService = GetRequiredService<IAttributeDefinitionAppService>();
        _productAppService = GetRequiredService<IProductAppService>();
        _brandAppService = GetRequiredService<IBrandAppService>();
        _attributeDefinitionRepository = GetRequiredService<IRepository<AttributeDefinition, Guid>>();
        _attributeDefinitionTranslationRepository = GetRequiredService<IRepository<AttributeDefinitionTranslation, Guid>>();
        _productTypeRepository = GetRequiredService<IRepository<ProductType, Guid>>();
        _productTypeAttributeRuleRepository = GetRequiredService<IRepository<ProductTypeAttributeRule, Guid>>();
    }

    [Fact]
    public async Task Should_Throw_DynamicAttributeRequired_When_Required_Key_Missing()
    {
        var brand = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
        {
            Name = $"AttrVal Brand {Guid.NewGuid():N}",
            Slug = $"attr-val-brand-{Guid.NewGuid():N}",
            IsActive = true,
            Translations = new List<BrandTranslationDto>
            {
                new() { Language = "en", Name = "AttrVal Brand" }
            }
        }));

        var productTypeId = Guid.NewGuid();
        var definitionId = Guid.NewGuid();
        await WithUnitOfWorkAsync(async () =>
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
            {
                var productType = new ProductType(
                    productTypeId,
                    $"ATTR_VAL_TYPE_{Guid.NewGuid():N}",
                    "Attr Validation Type");
                productType.SetTranslations(
                    new[]
                    {
                        new ProductTypeTranslation(Guid.NewGuid(), productTypeId, "en", "Attr Validation Type")
                    },
                    "en");
                await _productTypeRepository.InsertAsync(productType, autoSave: true);

                var attrDef = new AttributeDefinition(
                    definitionId,
                    $"part_number_{Guid.NewGuid():N}",
                    AttributeDefinitionDataType.Text,
                    isRequired: true);
                attrDef.Publish();
                await _attributeDefinitionRepository.InsertAsync(attrDef, autoSave: true);
                await _attributeDefinitionTranslationRepository.InsertAsync(new AttributeDefinitionTranslation(
                    Guid.NewGuid(),
                    definitionId,
                    "en",
                    "Part number",
                    null),
                    autoSave: true);

                await _productTypeAttributeRuleRepository.InsertAsync(
                    new ProductTypeAttributeRule(Guid.NewGuid(), productTypeId, definitionId, 10),
                    autoSave: true);
            }
        });

        // ValidateDynamicAttributesAsync runs per variant, not on product-level DynamicAttributes.
        var ex = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"ATTR-MISS-{Guid.NewGuid():N}",
            Name = "Missing required dynamic attribute",
            BrandId = brand.Id,
            ProductTypeId = productTypeId,
            DynamicAttributes = new Dictionary<string, object?>(),
            Translations = new List<ProductTranslationDto>
            {
                new() { Language = "en", Name = "Missing required dynamic attribute" }
            },
            Variants = new List<CreateProductVariantDto>
            {
                new()
                {
                    Sku = $"ATTR-MISS-SKU-{Guid.NewGuid():N}",
                    Price = 1m,
                    Quantity = 1,
                    DynamicAttributes = new Dictionary<string, object?>()
                }
            }
        })));

        ex.Code.ShouldBe("ECommerce:DynamicAttributeRequired");
    }

    [Fact]
    public async Task Should_Expose_Fallback_DisplayName_When_Current_Ui_Culture_Has_No_Translation()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        var definitionId = Guid.NewGuid();
        try
        {
            await WithDefaultTestTenantAsync(async () =>
            {
                await _attributeDefinitionRepository.InsertAsync(new AttributeDefinition(
                    definitionId,
                    $"metric_label_{Guid.NewGuid():N}",
                    AttributeDefinitionDataType.Text));
                await _attributeDefinitionTranslationRepository.InsertAsync(new AttributeDefinitionTranslation(
                    Guid.NewGuid(),
                    definitionId,
                    "en",
                    "Metric EN",
                    "Desc EN"));
            });

            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var readInFrench = await WithDefaultTestTenantAsync(() => _attributeDefinitionAppService.GetAsync(definitionId));

            readInFrench.DisplayName.ShouldBeNull();
            readInFrench.FallbackDisplayName.ShouldBe("Metric EN");
            readInFrench.FallbackDisplayNameLanguage.ShouldBe("en");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;

            await WithDefaultTestTenantAsync(() => _attributeDefinitionAppService.DeleteAsync(definitionId));
        }
    }
}
