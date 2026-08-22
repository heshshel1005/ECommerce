using ECommerce.Catalog;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace ECommerce;

[DependsOn(
    typeof(ECommerceApplicationModule),
    typeof(ECommerceDomainTestModule)
)]
public class ECommerceApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IProductMediaFileStorage, FakeProductMediaFileStorage>();
    }
}
