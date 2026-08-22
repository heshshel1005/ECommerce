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

public abstract class ProductAppServiceDynamicAttributeValidationTests<TStartupModule> : ECommerceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IProductAppService _productAppService;
    private readonly IBrandAppService _brandAppService;
    private readonly IRepository<ProductType, Guid> _productTypeRepository;
    private readonly IRepository<AttributeDefinition, Guid> _attributeDefinitionRepository;
    private readonly IRepository<ProductTypeAttributeRule, Guid> _productTypeAttributeRuleRepository;

    protected ProductAppServiceDynamicAttributeValidationTests()
    {
        _productAppService = GetRequiredService<IProductAppService>();
        _brandAppService = GetRequiredService<IBrandAppService>();
        _productTypeRepository = GetRequiredService<IRepository<ProductType, Guid>>();
        _attributeDefinitionRepository = GetRequiredService<IRepository<AttributeDefinition, Guid>>();
        _productTypeAttributeRuleRepository = GetRequiredService<IRepository<ProductTypeAttributeRule, Guid>>();
    }

    [Fact]
    public async Task Should_Validate_Dynamic_Attributes_For_Positive_And_Negative_Paths()
    {
        var fixture = await CreateValidationFixtureAsync();

        ProductDto? createdProduct = null;
        try
        {
            createdProduct = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
            {
                ProductNumber = $"VAL-OK-{Guid.NewGuid():N}",
                Name = "Validation Positive Product",
                BrandId = fixture.BrandId,
                ProductTypeId = fixture.ProductTypeId,
                DynamicAttributes = new Dictionary<string, object?>
                {
                    ["part_number"] = "PN-100",
                    ["condition"] = "new",
                    ["gtin_upc"] = "123456789012",
                    ["weight_kg"] = 5.25m
                },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "Validation Positive Product" }
                }
            }));

            createdProduct.Id.ShouldNotBe(Guid.Empty);

            var requiredException = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
            {
                ProductNumber = $"VAL-REQ-{Guid.NewGuid():N}",
                Name = "Validation Missing Required Product",
                BrandId = fixture.BrandId,
                ProductTypeId = fixture.ProductTypeId,
                DynamicAttributes = new Dictionary<string, object?>
                {
                    ["condition"] = "new",
                    ["gtin_upc"] = "123456789012",
                    ["weight_kg"] = 5
                },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "Validation Missing Required Product" }
                }
            })));
            requiredException.Code.ShouldBe("ECommerce:DynamicAttributeRequired");

            var enumException = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
            {
                ProductNumber = $"VAL-ENUM-{Guid.NewGuid():N}",
                Name = "Validation Invalid Enum Product",
                BrandId = fixture.BrandId,
                ProductTypeId = fixture.ProductTypeId,
                DynamicAttributes = new Dictionary<string, object?>
                {
                    ["part_number"] = "PN-101",
                    ["condition"] = "broken",
                    ["gtin_upc"] = "123456789012",
                    ["weight_kg"] = 5
                },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "Validation Invalid Enum Product" }
                }
            })));
            enumException.Code.ShouldBe("ECommerce:DynamicAttributeInvalidAllowedValue");

            var patternException = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
            {
                ProductNumber = $"VAL-PATTERN-{Guid.NewGuid():N}",
                Name = "Validation Invalid Pattern Product",
                BrandId = fixture.BrandId,
                ProductTypeId = fixture.ProductTypeId,
                DynamicAttributes = new Dictionary<string, object?>
                {
                    ["part_number"] = "PN-102",
                    ["condition"] = "used",
                    ["gtin_upc"] = "ABC-INVALID",
                    ["weight_kg"] = 5
                },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "Validation Invalid Pattern Product" }
                }
            })));
            patternException.Code.ShouldBe("ECommerce:DynamicAttributeRegexMismatch");

            var rangeException = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
            {
                ProductNumber = $"VAL-RANGE-{Guid.NewGuid():N}",
                Name = "Validation Out Of Range Product",
                BrandId = fixture.BrandId,
                ProductTypeId = fixture.ProductTypeId,
                DynamicAttributes = new Dictionary<string, object?>
                {
                    ["part_number"] = "PN-103",
                    ["condition"] = "remanufactured",
                    ["gtin_upc"] = "123456789012",
                    ["weight_kg"] = 99
                },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "Validation Out Of Range Product" }
                }
            })));
            rangeException.Code.ShouldBe("ECommerce:DynamicAttributeRangeViolation");
        }
        finally
        {
            if (createdProduct != null)
            {
                await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(createdProduct.Id));
            }
        }
    }

    [Fact]
    public async Task Should_Throw_Culture_Invariant_BusinessException_Codes_When_Ui_Culture_Is_Not_English()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        var fixture = await CreateValidationFixtureAsync();
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var ex = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
            {
                ProductNumber = $"VAL-CULTURE-{Guid.NewGuid():N}",
                Name = "Culture invariant validation codes",
                BrandId = fixture.BrandId,
                ProductTypeId = fixture.ProductTypeId,
                DynamicAttributes = new Dictionary<string, object?>
                {
                    ["condition"] = "new",
                    ["gtin_upc"] = "123456789012",
                    ["weight_kg"] = 5
                },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "Culture invariant validation codes" }
                }
            })));

            ex.Code.ShouldBe("ECommerce:DynamicAttributeRequired");
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
            Name = $"Validation Brand {Guid.NewGuid():N}",
            Slug = $"validation-brand-{Guid.NewGuid():N}",
            IsActive = true,
            Translations = new List<BrandTranslationDto>
            {
                new() { Language = "en", Name = "Validation Brand" }
            }
        }));

        var productTypeId = await WithUnitOfWorkAsync(async () =>
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
            {
                var ptId = Guid.NewGuid();
                var productType = new ProductType(
                    ptId,
                    $"VALIDATION_TYPE_{Guid.NewGuid():N}",
                    "Validation Type");
                productType.SetTranslations(
                    new[]
                    {
                        new ProductTypeTranslation(Guid.NewGuid(), ptId, "en", "Validation Type")
                    },
                    "en");
                await _productTypeRepository.InsertAsync(productType, autoSave: true);

                var partNumber = await CreateDefinitionAsync("part_number", isRequired: true);
                var condition = await CreateDefinitionAsync("condition", allowedValuesJson: "[\"new\",\"used\",\"remanufactured\"]", isRequired: true);
                var gtin = await CreateDefinitionAsync("gtin_upc", regexPattern: "^\\d{12,14}$", isRecommended: true);
                var weight = await CreateDefinitionAsync("weight_kg", minValue: 0.1m, maxValue: 10m, isRecommended: true);

                await CreateRuleAsync(ptId, partNumber.Id, 10);
                await CreateRuleAsync(ptId, condition.Id, 20);
                await CreateRuleAsync(ptId, gtin.Id, 30);
                await CreateRuleAsync(ptId, weight.Id, 40);

                return ptId;
            }
        });

        return new ValidationFixture(productTypeId, brand.Id);
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

    private async Task CreateRuleAsync(
        Guid productTypeId,
        Guid attributeDefinitionId,
        int order,
        string? conditionalAttributeKey = null,
        ProductTypeRuleConditionOperator? conditionalOperator = null,
        string? conditionalExpectedValue = null)
    {
        await WithDefaultTestTenantAsync(() => _productTypeAttributeRuleRepository.InsertAsync(
            new ProductTypeAttributeRule(
                Guid.NewGuid(),
                productTypeId,
                attributeDefinitionId,
                order,
                conditionalAttributeKey,
                conditionalOperator,
                conditionalExpectedValue),
            autoSave: true));
    }

    [Fact]
    public async Task Should_Not_Require_Conditional_Attribute_When_Condition_Is_False()
    {
        var fixture = await CreateChannelAndPoConditionalFixtureAsync();
        var created = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"COND-OFF-{Guid.NewGuid():N}",
            Name = "Conditional off",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?> { ["channel"] = "retail" },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "Conditional off" } }
        }));
        created.Id.ShouldNotBe(Guid.Empty);
        await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(created.Id));
    }

    [Fact]
    public async Task Should_Require_Conditional_Attribute_When_Condition_Is_True()
    {
        var fixture = await CreateChannelAndPoConditionalFixtureAsync();
        var ex = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"COND-ON-{Guid.NewGuid():N}",
            Name = "Conditional on",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?> { ["channel"] = "b2b" },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "Conditional on" } }
        })));
        ex.Code.ShouldBe("ECommerce:DynamicAttributeRequired");
    }

    [Fact]
    public async Task Should_Allow_Conditional_Attribute_When_Condition_Is_True_And_Value_Present()
    {
        var fixture = await CreateChannelAndPoConditionalFixtureAsync();
        var created = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"COND-OK-{Guid.NewGuid():N}",
            Name = "Conditional ok",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?>
            {
                ["channel"] = "b2b",
                ["po_number"] = "PO-1"
            },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "Conditional ok" } }
        }));
        created.Id.ShouldNotBe(Guid.Empty);
        await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(created.Id));
    }

    [Fact]
    public async Task Should_Still_Validate_Format_When_Condition_Is_False_But_Value_Is_Present()
    {
        var fixture = await CreateChannelAndPoConditionalFixtureAsync(poAllowedValuesJson: "[\"PO-OK\"]");
        var ex = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"COND-FMT-{Guid.NewGuid():N}",
            Name = "Conditional format",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?>
            {
                ["channel"] = "retail",
                ["po_number"] = "BAD"
            },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "Conditional format" } }
        })));
        ex.Code.ShouldBe("ECommerce:DynamicAttributeInvalidAllowedValue");
    }

    [Fact]
    public async Task Should_Evaluate_NotEquals_Condition_Operator()
    {
        var fixture = await CreateNotEqualsConditionalFixtureAsync();
        var ok = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"NE-OK-{Guid.NewGuid():N}",
            Name = "NotEquals ok",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?> { ["tier"] = "retail" },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "NotEquals ok" } }
        }));
        await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(ok.Id));

        var ex = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"NE-FAIL-{Guid.NewGuid():N}",
            Name = "NotEquals fail",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?> { ["tier"] = "enterprise" },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "NotEquals fail" } }
        })));
        ex.Code.ShouldBe("ECommerce:DynamicAttributeRequired");
    }

    [Fact]
    public async Task Should_Evaluate_Contains_Condition_Operator()
    {
        var fixture = await CreateContainsConditionalFixtureAsync();
        var ok = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"CT-OK-{Guid.NewGuid():N}",
            Name = "Contains ok",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?> { ["notes"] = "standard" },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "Contains ok" } }
        }));
        await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(ok.Id));

        var ex = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"CT-FAIL-{Guid.NewGuid():N}",
            Name = "Contains fail",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?> { ["notes"] = "urgent order" },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "Contains fail" } }
        })));
        ex.Code.ShouldBe("ECommerce:DynamicAttributeRequired");
    }

    [Fact]
    public async Task Should_Evaluate_NotContains_Condition_Operator()
    {
        var fixture = await CreateNotContainsConditionalFixtureAsync();
        var ok = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"NC-OK-{Guid.NewGuid():N}",
            Name = "NotContains ok",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?> { ["notes"] = "please skip review" },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "NotContains ok" } }
        }));
        await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(ok.Id));

        var ex = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"NC-FAIL-{Guid.NewGuid():N}",
            Name = "NotContains fail",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?> { ["notes"] = "normal" },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "NotContains fail" } }
        })));
        ex.Code.ShouldBe("ECommerce:DynamicAttributeRequired");
    }

    [Fact]
    public async Task Should_Treat_Missing_Condition_Driver_As_Empty_For_Equals()
    {
        var fixture = await CreateOptionalChannelAndPoConditionalFixtureAsync();
        var created = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"MISS-{Guid.NewGuid():N}",
            Name = "Missing driver",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?>(),
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "Missing driver" } }
        }));
        created.Id.ShouldNotBe(Guid.Empty);
        await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(created.Id));
    }

    [Fact]
    public async Task Should_Apply_Conditional_Validation_To_Variant_Dynamic_Attributes()
    {
        var fixture = await CreateChannelAndPoConditionalFixtureAsync();
        var ex = await Should.ThrowAsync<BusinessException>(() => WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
        {
            ProductNumber = $"VAR-COND-{Guid.NewGuid():N}",
            Name = "Variant conditional",
            BrandId = fixture.BrandId,
            ProductTypeId = fixture.ProductTypeId,
            DynamicAttributes = new Dictionary<string, object?> { ["channel"] = "retail" },
            Translations = new List<ProductTranslationDto> { new() { Language = "en", Name = "Variant conditional" } },
            Variants =
            {
                new CreateProductVariantDto
                {
                    Sku = $"SKU-{Guid.NewGuid():N}",
                    Quantity = 1,
                    DynamicAttributes = new Dictionary<string, object?> { ["channel"] = "b2b" }
                }
            }
        })));
        ex.Code.ShouldBe("ECommerce:DynamicAttributeRequired");
    }

    private async Task<ValidationFixture> CreateChannelAndPoConditionalFixtureAsync(string? poAllowedValuesJson = null)
    {
        var brand = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
        {
            Name = $"Cond Brand {Guid.NewGuid():N}",
            Slug = $"cond-brand-{Guid.NewGuid():N}",
            IsActive = true,
            Translations = new List<BrandTranslationDto> { new() { Language = "en", Name = "Cond Brand" } }
        }));

        var productTypeId = await WithUnitOfWorkAsync(async () =>
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
            {
                var ptId = Guid.NewGuid();
                var productType = new ProductType(ptId, $"COND_TYPE_{Guid.NewGuid():N}", "Cond Type");
                productType.SetTranslations(
                    new[] { new ProductTypeTranslation(Guid.NewGuid(), ptId, "en", "Cond Type") },
                    "en");
                await _productTypeRepository.InsertAsync(productType, autoSave: true);

                var channel = await CreateDefinitionAsync(
                    "channel",
                    allowedValuesJson: "[\"retail\",\"b2b\"]",
                    isRequired: true);
                var po = await CreateDefinitionAsync(
                    "po_number",
                    allowedValuesJson: poAllowedValuesJson,
                    isRequired: true);

                await CreateRuleAsync(ptId, channel.Id, 10);
                await CreateRuleAsync(
                    ptId,
                    po.Id,
                    20,
                    conditionalAttributeKey: "channel",
                    conditionalOperator: ProductTypeRuleConditionOperator.Equals,
                    conditionalExpectedValue: "b2b");

                return ptId;
            }
        });

        return new ValidationFixture(productTypeId, brand.Id);
    }

    private async Task<ValidationFixture> CreateOptionalChannelAndPoConditionalFixtureAsync()
    {
        var brand = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
        {
            Name = $"Opt Brand {Guid.NewGuid():N}",
            Slug = $"opt-brand-{Guid.NewGuid():N}",
            IsActive = true,
            Translations = new List<BrandTranslationDto> { new() { Language = "en", Name = "Opt Brand" } }
        }));

        var productTypeId = await WithUnitOfWorkAsync(async () =>
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
            {
                var ptId = Guid.NewGuid();
                var productType = new ProductType(ptId, $"OPT_TYPE_{Guid.NewGuid():N}", "Opt Type");
                productType.SetTranslations(
                    new[] { new ProductTypeTranslation(Guid.NewGuid(), ptId, "en", "Opt Type") },
                    "en");
                await _productTypeRepository.InsertAsync(productType, autoSave: true);

                var channel = await CreateDefinitionAsync("channel", allowedValuesJson: "[\"retail\",\"b2b\"]", isRequired: false);
                var po = await CreateDefinitionAsync("po_number", isRequired: true);
                await CreateRuleAsync(ptId, channel.Id, 10);
                await CreateRuleAsync(
                    ptId,
                    po.Id,
                    20,
                    conditionalAttributeKey: "channel",
                    conditionalOperator: ProductTypeRuleConditionOperator.Equals,
                    conditionalExpectedValue: "b2b");

                return ptId;
            }
        });

        return new ValidationFixture(productTypeId, brand.Id);
    }

    private async Task<ValidationFixture> CreateNotEqualsConditionalFixtureAsync()
    {
        var brand = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
        {
            Name = $"NE Brand {Guid.NewGuid():N}",
            Slug = $"ne-brand-{Guid.NewGuid():N}",
            IsActive = true,
            Translations = new List<BrandTranslationDto> { new() { Language = "en", Name = "NE Brand" } }
        }));

        var productTypeId = await WithUnitOfWorkAsync(async () =>
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
            {
                var ptId = Guid.NewGuid();
                var productType = new ProductType(ptId, $"NE_TYPE_{Guid.NewGuid():N}", "NE Type");
                productType.SetTranslations(
                    new[] { new ProductTypeTranslation(Guid.NewGuid(), ptId, "en", "NE Type") },
                    "en");
                await _productTypeRepository.InsertAsync(productType, autoSave: true);

                var tier = await CreateDefinitionAsync(
                    "tier",
                    allowedValuesJson: "[\"retail\",\"enterprise\"]",
                    isRequired: true);
                var contractId = await CreateDefinitionAsync("contract_id", isRequired: true);
                await CreateRuleAsync(ptId, tier.Id, 10);
                await CreateRuleAsync(
                    ptId,
                    contractId.Id,
                    20,
                    conditionalAttributeKey: "tier",
                    conditionalOperator: ProductTypeRuleConditionOperator.NotEquals,
                    conditionalExpectedValue: "retail");

                return ptId;
            }
        });

        return new ValidationFixture(productTypeId, brand.Id);
    }

    private async Task<ValidationFixture> CreateContainsConditionalFixtureAsync()
    {
        var brand = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
        {
            Name = $"CT Brand {Guid.NewGuid():N}",
            Slug = $"ct-brand-{Guid.NewGuid():N}",
            IsActive = true,
            Translations = new List<BrandTranslationDto> { new() { Language = "en", Name = "CT Brand" } }
        }));

        var productTypeId = await WithUnitOfWorkAsync(async () =>
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
            {
                var ptId = Guid.NewGuid();
                var productType = new ProductType(ptId, $"CT_TYPE_{Guid.NewGuid():N}", "CT Type");
                productType.SetTranslations(
                    new[] { new ProductTypeTranslation(Guid.NewGuid(), ptId, "en", "CT Type") },
                    "en");
                await _productTypeRepository.InsertAsync(productType, autoSave: true);

                var notes = await CreateDefinitionAsync("notes", isRequired: false);
                var ticket = await CreateDefinitionAsync("ticket_id", isRequired: true);
                await CreateRuleAsync(ptId, notes.Id, 10);
                await CreateRuleAsync(
                    ptId,
                    ticket.Id,
                    20,
                    conditionalAttributeKey: "notes",
                    conditionalOperator: ProductTypeRuleConditionOperator.Contains,
                    conditionalExpectedValue: "urgent");

                return ptId;
            }
        });

        return new ValidationFixture(productTypeId, brand.Id);
    }

    private async Task<ValidationFixture> CreateNotContainsConditionalFixtureAsync()
    {
        var brand = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
        {
            Name = $"NC Brand {Guid.NewGuid():N}",
            Slug = $"nc-brand-{Guid.NewGuid():N}",
            IsActive = true,
            Translations = new List<BrandTranslationDto> { new() { Language = "en", Name = "NC Brand" } }
        }));

        var productTypeId = await WithUnitOfWorkAsync(async () =>
        {
            var currentTenant = GetRequiredService<ICurrentTenant>();
            using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
            {
                var ptId = Guid.NewGuid();
                var productType = new ProductType(ptId, $"NC_TYPE_{Guid.NewGuid():N}", "NC Type");
                productType.SetTranslations(
                    new[] { new ProductTypeTranslation(Guid.NewGuid(), ptId, "en", "NC Type") },
                    "en");
                await _productTypeRepository.InsertAsync(productType, autoSave: true);

                var notes = await CreateDefinitionAsync("notes", isRequired: false);
                var approval = await CreateDefinitionAsync("approval_code", isRequired: true);
                await CreateRuleAsync(ptId, notes.Id, 10);
                await CreateRuleAsync(
                    ptId,
                    approval.Id,
                    20,
                    conditionalAttributeKey: "notes",
                    conditionalOperator: ProductTypeRuleConditionOperator.NotContains,
                    conditionalExpectedValue: "skip");

                return ptId;
            }
        });

        return new ValidationFixture(productTypeId, brand.Id);
    }

    private sealed record ValidationFixture(Guid ProductTypeId, Guid BrandId);
}
