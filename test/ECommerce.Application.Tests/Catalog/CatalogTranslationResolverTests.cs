using System.Collections.Generic;
using ECommerce.Localization;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;

namespace ECommerce.Catalog;

public abstract class CatalogTranslationResolverTests<TStartupModule> : ECommerceApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    [Fact]
    public void Resolve_Should_Prefer_Exact_Culture_Match()
    {
        var translations = new List<TestTranslation>
        {
            new("en", "Name EN"),
            new("en-US", "Name EN-US"),
            new("fr", "Name FR")
        };

        var resolved = CatalogTranslationResolver.Resolve(translations, "en-US", "fr");

        resolved.ShouldNotBeNull();
        resolved.Name.ShouldBe("Name EN-US");
    }

    [Fact]
    public void Resolve_Should_Fall_Back_To_Neutral_Culture()
    {
        var translations = new List<TestTranslation>
        {
            new("en", "Name EN"),
            new("fr", "Name FR")
        };

        var resolved = CatalogTranslationResolver.Resolve(translations, "en-GB", "fr");

        resolved.ShouldNotBeNull();
        resolved.Name.ShouldBe("Name EN");
    }

    [Fact]
    public void Resolve_Should_Fall_Back_To_Default_Language_When_Culture_Not_Found()
    {
        var translations = new List<TestTranslation>
        {
            new("de", "Name DE"),
            new("fr", "Name FR")
        };

        var resolved = CatalogTranslationResolver.Resolve(translations, "en-US", "fr");

        resolved.ShouldNotBeNull();
        resolved.Name.ShouldBe("Name FR");
    }

    [Fact]
    public void Resolve_Should_Fall_Back_To_First_Available_When_No_Fallback_Matches()
    {
        var translations = new List<TestTranslation>
        {
            new("de", "Name DE"),
            new("it", "Name IT")
        };

        var resolved = CatalogTranslationResolver.Resolve(translations, "en-US", "fr");

        resolved.ShouldNotBeNull();
        resolved.Name.ShouldBe("Name DE");
    }

    private sealed class TestTranslation : IEntityTranslation
    {
        public string Language { get; set; }

        public string Name { get; }

        public TestTranslation(string language, string name)
        {
            Language = language;
            Name = name;
        }
    }
}
