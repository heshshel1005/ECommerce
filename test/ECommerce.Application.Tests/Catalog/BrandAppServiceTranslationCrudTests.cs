using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Modularity;
using Xunit;

namespace ECommerce.Catalog;

public abstract class BrandAppServiceTranslationCrudTests<TStartupModule> : ECommerceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IBrandAppService _brandAppService;

    protected BrandAppServiceTranslationCrudTests()
    {
        _brandAppService = GetRequiredService<IBrandAppService>();
    }

    [Fact]
    public async Task Should_Perform_Brand_Translation_Crud_Through_AppService_Contract()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var created = await WithDefaultTestTenantAsync(() => _brandAppService.CreateAsync(new CreateBrandDto
            {
                Name = "Fallback Name",
                Slug = "translation-crud-brand",
                Description = "Fallback Description",
                IsActive = true,
                Translations = new List<BrandTranslationDto>
                {
                    new() { Language = "en", Name = "Brand EN", Description = "Description EN" },
                    new() { Language = "fr", Name = "Marque FR", Description = "Description FR" }
                }
            }));

            created.Name.ShouldBe("Marque FR");
            created.Description.ShouldBe("Description FR");

            var createdId = created.Id;

            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            var readInEnglish = await WithDefaultTestTenantAsync(() => _brandAppService.GetAsync(createdId));
            readInEnglish.Name.ShouldBe("Brand EN");
            readInEnglish.Description.ShouldBe("Description EN");

            var updated = await WithDefaultTestTenantAsync(() => _brandAppService.UpdateAsync(createdId, new UpdateBrandDto
            {
                Name = "Updated Fallback",
                Slug = "translation-crud-brand-updated",
                Description = "Updated Fallback Description",
                IsActive = false,
                Translations = new List<BrandTranslationDto>
                {
                    new() { Language = "en", Name = "Brand EN Updated", Description = "Description EN Updated" },
                    new() { Language = "de", Name = "Marke DE", Description = "Beschreibung DE" }
                }
            }));

            updated.Name.ShouldBe("Brand EN Updated");
            updated.IsActive.ShouldBeFalse();

            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            CultureInfo.CurrentUICulture = new CultureInfo("de-DE");

            var readInGerman = await WithDefaultTestTenantAsync(() => _brandAppService.GetAsync(createdId));
            readInGerman.Name.ShouldBe("Marke DE");
            readInGerman.Description.ShouldBe("Beschreibung DE");

            var list = await WithDefaultTestTenantAsync(() => _brandAppService.GetListAsync(isActive: false));
            list.ShouldContain(x => x.Id == createdId && x.Name == "Marke DE");

            await WithDefaultTestTenantAsync(() => _brandAppService.DeleteAsync(createdId));

            await Should.ThrowAsync<EntityNotFoundException>(
                () => WithDefaultTestTenantAsync(() => _brandAppService.GetAsync(createdId)));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
