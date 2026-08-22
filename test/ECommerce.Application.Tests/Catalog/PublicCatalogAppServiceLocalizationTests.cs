using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace ECommerce.Catalog;

public abstract class PublicCatalogAppServiceLocalizationTests<TStartupModule> : ECommerceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IPublicCatalogAppService _publicCatalogAppService;
    private readonly ICategoryAppService _categoryAppService;
    private readonly IBrandAppService _brandAppService;
    private readonly IBrandModelAppService _brandModelAppService;
    private readonly IProductAppService _productAppService;
    private readonly IRepository<ProductType, Guid> _productTypeRepository;
    private readonly IRepository<AttributeDefinition, Guid> _attributeDefinitionRepository;
    private readonly IRepository<AttributeDefinitionTranslation, Guid> _attributeDefinitionTranslationRepository;
    private readonly IRepository<AttributeOption, Guid> _attributeOptionRepository;
    private readonly IRepository<AttributeOptionTranslation, Guid> _attributeOptionTranslationRepository;
    private readonly IRepository<ProductTypeAttributeRule, Guid> _productTypeAttributeRuleRepository;

    protected PublicCatalogAppServiceLocalizationTests()
    {
        _publicCatalogAppService = GetRequiredService<IPublicCatalogAppService>();
        _categoryAppService = GetRequiredService<ICategoryAppService>();
        _brandAppService = GetRequiredService<IBrandAppService>();
        _brandModelAppService = GetRequiredService<IBrandModelAppService>();
        _productAppService = GetRequiredService<IProductAppService>();
        _productTypeRepository = GetRequiredService<IRepository<ProductType, Guid>>();
        _attributeDefinitionRepository = GetRequiredService<IRepository<AttributeDefinition, Guid>>();
        _attributeDefinitionTranslationRepository = GetRequiredService<IRepository<AttributeDefinitionTranslation, Guid>>();
        _attributeOptionRepository = GetRequiredService<IRepository<AttributeOption, Guid>>();
        _attributeOptionTranslationRepository = GetRequiredService<IRepository<AttributeOptionTranslation, Guid>>();
        _productTypeAttributeRuleRepository = GetRequiredService<IRepository<ProductTypeAttributeRule, Guid>>();
    }

    [Fact]
    public async Task Should_Return_Localized_Product_Data_In_Public_List_And_Detail()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        CategoryDto? category = null;
        BrandDto? brand = null;
        BrandModelDto? model = null;
        ProductDto? product = null;

        try
        {
            category = await WithDefaultTestTenantAsync(() => _categoryAppService.CreateAsync(new CreateCategoryDto
            {
                Name = "Category EN",
                Slug = $"public-localized-category-{Guid.NewGuid():N}",
                Translations = new List<CategoryTranslationDto>
                {
                    new() { Language = "en", Name = "Category EN" },
                    new() { Language = "fr", Name = "Categorie FR" }
                }
            }));

            brand = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
            {
                Name = "Brand EN",
                Slug = $"public-localized-brand-{Guid.NewGuid():N}",
                Description = "Brand Description EN",
                IsActive = true,
                Translations = new List<BrandTranslationDto>
                {
                    new() { Language = "en", Name = "Brand EN", Description = "Brand Description EN" },
                    new() { Language = "fr", Name = "Marque FR", Description = "Description Marque FR" }
                }
            }));

            model = await WithDefaultTestTenantAsync(() => _brandModelAppService.CreateAsync(new CreateBrandModelDto
            {
                BrandId = brand.Id,
                Name = "Model EN",
                Code = $"public-localized-model-{Guid.NewGuid():N}",
                IsActive = true,
                Translations = new List<BrandModelTranslationDto>
                {
                    new() { Language = "en", Name = "Model EN" },
                    new() { Language = "fr", Name = "Modele FR" }
                }
            }));

            product = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
            {
                ProductNumber = $"PUBLIC-LOC-{Guid.NewGuid():N}",
                Name = "Product EN",
                Description = "Product Description EN",
                CategoryId = category.Id,
                BrandId = brand.Id,
                ModelId = model.Id,
                IsPublished = true,
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "Product EN", Description = "Product Description EN" },
                    new() { Language = "fr", Name = "Produit FR", Description = "Description Produit FR" }
                }
            }));

            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var listInFrench = await WithDefaultTestTenantAsync(() => _publicCatalogAppService.GetProductListAsync(new PublicProductListRequestDto
            {
                MaxResultCount = 50
            }));
            var frenchItem = listInFrench.Items.Single(x => x.Id == product.Id);
            frenchItem.Name.ShouldBe("Produit FR");
            frenchItem.CategoryName.ShouldBe("Categorie FR");
            frenchItem.BrandName.ShouldBe("Marque FR");
            frenchItem.ModelName.ShouldBe("Modele FR");

            var detailInFrench = await WithDefaultTestTenantAsync(() => _publicCatalogAppService.GetProductDetailAsync(product.Id));
            detailInFrench.Name.ShouldBe("Produit FR");
            detailInFrench.Description.ShouldBe("Description Produit FR");
            detailInFrench.BrandName.ShouldBe("Marque FR");
            detailInFrench.ModelName.ShouldBe("Modele FR");

            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            var listInEnglish = await WithDefaultTestTenantAsync(() => _publicCatalogAppService.GetProductListAsync(new PublicProductListRequestDto
            {
                MaxResultCount = 50
            }));
            var englishItem = listInEnglish.Items.Single(x => x.Id == product.Id);
            englishItem.Name.ShouldBe("Product EN");
            englishItem.CategoryName.ShouldBe("Category EN");
            englishItem.BrandName.ShouldBe("Brand EN");
            englishItem.ModelName.ShouldBe("Model EN");

            var detailInEnglish = await WithDefaultTestTenantAsync(() => _publicCatalogAppService.GetProductDetailAsync(product.Id));
            detailInEnglish.Name.ShouldBe("Product EN");
            detailInEnglish.Description.ShouldBe("Product Description EN");
            detailInEnglish.BrandName.ShouldBe("Brand EN");
            detailInEnglish.ModelName.ShouldBe("Model EN");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;

            if (product != null)
            {
                await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(product.Id));
            }

            if (model != null)
            {
                await WithDefaultTestTenantAsync(() => _brandModelAppService.DeleteAsync(model.Id));
            }

            if (brand != null)
            {
                await WithDefaultTestTenantAsync(() => _brandAppService.DeleteAsync(brand.Id));
            }

            if (category != null)
            {
                await WithDefaultTestTenantAsync(() => _categoryAppService.DeleteAsync(category.Id));
            }
        }
    }

    [Fact]
    public async Task Should_Return_Localized_Dynamic_Filter_Facets_For_Current_Ui_Culture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        CategoryDto? category = null;
        BrandDto? brand = null;
        BrandModelDto? model = null;
        ProductDto? product = null;
        var productTypeId = Guid.NewGuid();
        var definitionId = Guid.NewGuid();
        var optionBlueId = Guid.NewGuid();
        var optionRedId = Guid.NewGuid();
        var ruleId = Guid.NewGuid();
        var attributeKey = $"ff_loc_{Guid.NewGuid():N}";

        try
        {
            category = await WithDefaultTestTenantAsync(() => _categoryAppService.CreateAsync(new CreateCategoryDto
            {
                Name = "Facet Category EN",
                Slug = $"facet-loc-cat-{Guid.NewGuid():N}",
                Translations = new List<CategoryTranslationDto>
                {
                    new() { Language = "en", Name = "Facet Category EN" }
                }
            }));

            brand = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
            {
                Name = "Facet Brand EN",
                Slug = $"facet-loc-brand-{Guid.NewGuid():N}",
                IsActive = true,
                Translations = new List<BrandTranslationDto>
                {
                    new() { Language = "en", Name = "Facet Brand EN" }
                }
            }));

            model = await WithDefaultTestTenantAsync(() => _brandModelAppService.CreateAsync(new CreateBrandModelDto
            {
                BrandId = brand.Id,
                Name = "Facet Model EN",
                Code = $"facet-loc-model-{Guid.NewGuid():N}",
                IsActive = true,
                Translations = new List<BrandModelTranslationDto>
                {
                    new() { Language = "en", Name = "Facet Model EN" }
                }
            }));

            await WithDefaultTestTenantAsync(async () =>
            {
                var productType = new ProductType(
                    productTypeId,
                    $"FF_TYPE_{Guid.NewGuid():N}",
                    "Facet Product Type");
                productType.SetTranslations(
                    new[]
                    {
                        new ProductTypeTranslation(Guid.NewGuid(), productTypeId, "en", "Facet Product Type")
                    },
                    "en");
                await _productTypeRepository.InsertAsync(productType, autoSave: true);

                var colorDef = new AttributeDefinition(
                    definitionId,
                    attributeKey,
                    AttributeDefinitionDataType.Enum,
                    "[\"blue\",\"red\"]",
                    isRequired: false);
                colorDef.Publish();
                await _attributeDefinitionRepository.InsertAsync(colorDef, autoSave: true);

                await _attributeDefinitionTranslationRepository.InsertAsync(
                    new AttributeDefinitionTranslation(Guid.NewGuid(), definitionId, "en", "Color", null),
                    autoSave: true);
                await _attributeDefinitionTranslationRepository.InsertAsync(
                    new AttributeDefinitionTranslation(Guid.NewGuid(), definitionId, "fr", "Couleur", null),
                    autoSave: true);

                await _attributeOptionRepository.InsertAsync(
                    new AttributeOption(optionBlueId, definitionId, "blue", displayOrder: 0),
                    autoSave: true);
                await _attributeOptionRepository.InsertAsync(
                    new AttributeOption(optionRedId, definitionId, "red", displayOrder: 1),
                    autoSave: true);

                await _attributeOptionTranslationRepository.InsertAsync(
                    new AttributeOptionTranslation(Guid.NewGuid(), optionBlueId, "en", "Blue"),
                    autoSave: true);
                await _attributeOptionTranslationRepository.InsertAsync(
                    new AttributeOptionTranslation(Guid.NewGuid(), optionBlueId, "fr", "Bleu"),
                    autoSave: true);
                await _attributeOptionTranslationRepository.InsertAsync(
                    new AttributeOptionTranslation(Guid.NewGuid(), optionRedId, "en", "Red"),
                    autoSave: true);
                await _attributeOptionTranslationRepository.InsertAsync(
                    new AttributeOptionTranslation(Guid.NewGuid(), optionRedId, "fr", "Rouge"),
                    autoSave: true);

                await _productTypeAttributeRuleRepository.InsertAsync(
                    new ProductTypeAttributeRule(ruleId, productTypeId, definitionId, 10),
                    autoSave: true);
            });

            product = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
            {
                ProductNumber = $"FACET-LOC-{Guid.NewGuid():N}",
                Name = "Facet Product",
                CategoryId = category.Id,
                BrandId = brand.Id,
                ModelId = model.Id,
                ProductTypeId = productTypeId,
                IsPublished = true,
                DynamicAttributes = new Dictionary<string, object?> { [attributeKey] = "blue" },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "Facet Product" }
                }
            }));

            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var filters = await WithDefaultTestTenantAsync(() => _publicCatalogAppService.GetFilterOptionsAsync(
                new CatalogFilterOptionsRequestDto { CategoryId = category.Id }));

            var facet = filters.Attributes.Single(x => x.Key == attributeKey);
            facet.DisplayName.ShouldBe("Couleur");
            facet.DisplayNameLanguage.ShouldBe("fr");
            facet.FallbackDisplayName.ShouldBeNull();
            facet.FallbackDisplayNameLanguage.ShouldBeNull();

            var blue = facet.LocalizedValues.Single(v => v.Value == "blue");
            blue.DisplayName.ShouldBe("Bleu");
            blue.DisplayNameLanguage.ShouldBe("fr");
            blue.FallbackDisplayName.ShouldBeNull();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;

            if (product != null)
            {
                await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(product.Id));
            }

            await WithDefaultTestTenantAsync(async () =>
            {
                await _productTypeAttributeRuleRepository.DeleteAsync(ruleId);
                await _attributeOptionTranslationRepository.DeleteManyAsync(
                    await _attributeOptionTranslationRepository.GetListAsync(x =>
                        x.AttributeOptionId == optionBlueId || x.AttributeOptionId == optionRedId));
                await _attributeOptionRepository.DeleteAsync(optionBlueId);
                await _attributeOptionRepository.DeleteAsync(optionRedId);
                await _attributeDefinitionTranslationRepository.DeleteManyAsync(
                    await _attributeDefinitionTranslationRepository.GetListAsync(x => x.AttributeDefinitionId == definitionId));
                await _attributeDefinitionRepository.DeleteAsync(definitionId);
                await _productTypeRepository.DeleteAsync(productTypeId);
            });

            if (model != null)
            {
                await WithDefaultTestTenantAsync(() => _brandModelAppService.DeleteAsync(model.Id));
            }

            if (brand != null)
            {
                await WithDefaultTestTenantAsync(() => _brandAppService.DeleteAsync(brand.Id));
            }

            if (category != null)
            {
                await WithDefaultTestTenantAsync(() => _categoryAppService.DeleteAsync(category.Id));
            }
        }
    }

    [Fact]
    public async Task Should_Expose_Fallback_Filter_Facet_Labels_When_Current_Ui_Culture_Has_No_Translation()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        CategoryDto? category = null;
        BrandDto? brand = null;
        BrandModelDto? model = null;
        ProductDto? product = null;
        var productTypeId = Guid.NewGuid();
        var definitionId = Guid.NewGuid();
        var optionBlueId = Guid.NewGuid();
        var ruleId = Guid.NewGuid();
        var attributeKey = $"ff_fb_{Guid.NewGuid():N}";

        try
        {
            category = await WithDefaultTestTenantAsync(() => _categoryAppService.CreateAsync(new CreateCategoryDto
            {
                Name = "Facet FB Category EN",
                Slug = $"facet-fb-cat-{Guid.NewGuid():N}",
                Translations = new List<CategoryTranslationDto>
                {
                    new() { Language = "en", Name = "Facet FB Category EN" }
                }
            }));

            brand = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
            {
                Name = "Facet FB Brand EN",
                Slug = $"facet-fb-brand-{Guid.NewGuid():N}",
                IsActive = true,
                Translations = new List<BrandTranslationDto>
                {
                    new() { Language = "en", Name = "Facet FB Brand EN" }
                }
            }));

            model = await WithDefaultTestTenantAsync(() => _brandModelAppService.CreateAsync(new CreateBrandModelDto
            {
                BrandId = brand.Id,
                Name = "Facet FB Model EN",
                Code = $"facet-fb-model-{Guid.NewGuid():N}",
                IsActive = true,
                Translations = new List<BrandModelTranslationDto>
                {
                    new() { Language = "en", Name = "Facet FB Model EN" }
                }
            }));

            await WithDefaultTestTenantAsync(async () =>
            {
                var productType = new ProductType(
                    productTypeId,
                    $"FF_FB_TYPE_{Guid.NewGuid():N}",
                    "Facet Fallback Type");
                productType.SetTranslations(
                    new[]
                    {
                        new ProductTypeTranslation(Guid.NewGuid(), productTypeId, "en", "Facet Fallback Type")
                    },
                    "en");
                await _productTypeRepository.InsertAsync(productType, autoSave: true);

                var colorDefFb = new AttributeDefinition(
                    definitionId,
                    attributeKey,
                    AttributeDefinitionDataType.Enum,
                    "[\"blue\"]",
                    isRequired: false);
                colorDefFb.Publish();
                await _attributeDefinitionRepository.InsertAsync(colorDefFb, autoSave: true);

                await _attributeDefinitionTranslationRepository.InsertAsync(
                    new AttributeDefinitionTranslation(Guid.NewGuid(), definitionId, "en", "Color EN", null),
                    autoSave: true);

                await _attributeOptionRepository.InsertAsync(
                    new AttributeOption(optionBlueId, definitionId, "blue", displayOrder: 0),
                    autoSave: true);
                await _attributeOptionTranslationRepository.InsertAsync(
                    new AttributeOptionTranslation(Guid.NewGuid(), optionBlueId, "en", "Blue EN"),
                    autoSave: true);

                await _productTypeAttributeRuleRepository.InsertAsync(
                    new ProductTypeAttributeRule(ruleId, productTypeId, definitionId, 10),
                    autoSave: true);
            });

            product = await WithDefaultTestTenantAsync(() => _productAppService.CreateAsync(new CreateProductDto
            {
                ProductNumber = $"FACET-FB-{Guid.NewGuid():N}",
                Name = "Facet Fallback Product",
                CategoryId = category.Id,
                BrandId = brand.Id,
                ModelId = model.Id,
                ProductTypeId = productTypeId,
                IsPublished = true,
                DynamicAttributes = new Dictionary<string, object?> { [attributeKey] = "blue" },
                Translations = new List<ProductTranslationDto>
                {
                    new() { Language = "en", Name = "Facet Fallback Product" }
                }
            }));

            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var filters = await WithDefaultTestTenantAsync(() => _publicCatalogAppService.GetFilterOptionsAsync(
                new CatalogFilterOptionsRequestDto { CategoryId = category.Id }));

            var facet = filters.Attributes.Single(x => x.Key == attributeKey);
            facet.DisplayName.ShouldBeNull();
            facet.FallbackDisplayName.ShouldBe("Color EN");
            facet.FallbackDisplayNameLanguage.ShouldBe("en");

            var blue = facet.LocalizedValues.Single(v => v.Value == "blue");
            blue.DisplayName.ShouldBeNull();
            blue.FallbackDisplayName.ShouldBe("Blue EN");
            blue.FallbackDisplayNameLanguage.ShouldBe("en");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;

            if (product != null)
            {
                await WithDefaultTestTenantAsync(() => _productAppService.DeleteAsync(product.Id));
            }

            await WithDefaultTestTenantAsync(async () =>
            {
                await _productTypeAttributeRuleRepository.DeleteAsync(ruleId);
                await _attributeOptionTranslationRepository.DeleteManyAsync(
                    await _attributeOptionTranslationRepository.GetListAsync(x => x.AttributeOptionId == optionBlueId));
                await _attributeOptionRepository.DeleteAsync(optionBlueId);
                await _attributeDefinitionTranslationRepository.DeleteManyAsync(
                    await _attributeDefinitionTranslationRepository.GetListAsync(x => x.AttributeDefinitionId == definitionId));
                await _attributeDefinitionRepository.DeleteAsync(definitionId);
                await _productTypeRepository.DeleteAsync(productTypeId);
            });

            if (model != null)
            {
                await WithDefaultTestTenantAsync(() => _brandModelAppService.DeleteAsync(model.Id));
            }

            if (brand != null)
            {
                await WithDefaultTestTenantAsync(() => _brandAppService.DeleteAsync(brand.Id));
            }

            if (category != null)
            {
                await WithDefaultTestTenantAsync(() => _categoryAppService.DeleteAsync(category.Id));
            }
        }
    }
}
