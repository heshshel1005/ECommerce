using Microsoft.Extensions.Localization;
using ECommerce.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace ECommerce;

[Dependency(ReplaceServices = true)]
public class ECommerceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<ECommerceResource> _localizer;

    public ECommerceBrandingProvider(IStringLocalizer<ECommerceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
