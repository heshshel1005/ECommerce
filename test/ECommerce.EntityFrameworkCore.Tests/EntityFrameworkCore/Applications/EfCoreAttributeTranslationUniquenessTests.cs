using System;
using System.Threading.Tasks;
using ECommerce.Catalog;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace ECommerce.EntityFrameworkCore.Applications;

[Collection(ECommerceTestConsts.CollectionDefinitionName)]
public class EfCoreAttributeTranslationUniquenessTests : ECommerceEntityFrameworkCoreTestBase
{
    private readonly IRepository<AttributeDefinition, Guid> _attributeDefinitionRepository;
    private readonly IRepository<AttributeDefinitionTranslation, Guid> _attributeDefinitionTranslationRepository;
    private readonly IRepository<AttributeOption, Guid> _attributeOptionRepository;
    private readonly IRepository<AttributeOptionTranslation, Guid> _attributeOptionTranslationRepository;

    public EfCoreAttributeTranslationUniquenessTests()
    {
        _attributeDefinitionRepository = GetRequiredService<IRepository<AttributeDefinition, Guid>>();
        _attributeDefinitionTranslationRepository = GetRequiredService<IRepository<AttributeDefinitionTranslation, Guid>>();
        _attributeOptionRepository = GetRequiredService<IRepository<AttributeOption, Guid>>();
        _attributeOptionTranslationRepository = GetRequiredService<IRepository<AttributeOptionTranslation, Guid>>();
    }

    [Fact]
    public async Task Should_Enforce_Unique_Language_Per_Attribute_Definition_And_Option()
    {
        var definitionId = Guid.NewGuid();
        await WithDefaultTestTenantAsync(() => _attributeDefinitionRepository.InsertAsync(
            new AttributeDefinition(
                definitionId,
                $"condition_{Guid.NewGuid():N}",
                AttributeDefinitionDataType.Enum,
                "[\"new\",\"used\"]",
                isRequired: true),
            autoSave: true));

        await WithDefaultTestTenantAsync(() => _attributeDefinitionTranslationRepository.InsertAsync(
            new AttributeDefinitionTranslation(Guid.NewGuid(), definitionId, "en", "Condition"),
            autoSave: true));

        var duplicateDefinitionLanguage = await Should.ThrowAsync<DbUpdateException>(() =>
            WithDefaultTestTenantAsync(() => _attributeDefinitionTranslationRepository.InsertAsync(
                new AttributeDefinitionTranslation(Guid.NewGuid(), definitionId, "en", "Condition Duplicate"),
                autoSave: true)));
        duplicateDefinitionLanguage.ShouldNotBeNull();

        var optionId = Guid.NewGuid();
        await WithDefaultTestTenantAsync(() => _attributeOptionRepository.InsertAsync(
            new AttributeOption(optionId, definitionId, "new", displayOrder: 0),
            autoSave: true));

        await WithDefaultTestTenantAsync(() => _attributeOptionTranslationRepository.InsertAsync(
            new AttributeOptionTranslation(Guid.NewGuid(), optionId, "en", "New"),
            autoSave: true));

        var duplicateOptionLanguage = await Should.ThrowAsync<DbUpdateException>(() =>
            WithDefaultTestTenantAsync(() => _attributeOptionTranslationRepository.InsertAsync(
                new AttributeOptionTranslation(Guid.NewGuid(), optionId, "en", "Brand New"),
                autoSave: true)));
        duplicateOptionLanguage.ShouldNotBeNull();
    }

    [Fact]
    public async Task Should_Enforce_Unique_Attribute_Option_Value_Per_Definition_And_Tenant()
    {
        var definitionId = Guid.NewGuid();
        await WithDefaultTestTenantAsync(() => _attributeDefinitionRepository.InsertAsync(
            new AttributeDefinition(
                definitionId,
                $"opt_value_uniq_{Guid.NewGuid():N}",
                AttributeDefinitionDataType.Enum,
                "[\"same\"]",
                isRequired: false),
            autoSave: true));

        await WithDefaultTestTenantAsync(() => _attributeOptionRepository.InsertAsync(
            new AttributeOption(Guid.NewGuid(), definitionId, "same", displayOrder: 0),
            autoSave: true));

        var duplicateValue = await Should.ThrowAsync<DbUpdateException>(() =>
            WithDefaultTestTenantAsync(() => _attributeOptionRepository.InsertAsync(
                new AttributeOption(Guid.NewGuid(), definitionId, "same", displayOrder: 1),
                autoSave: true)));
        duplicateValue.ShouldNotBeNull();
    }
}
