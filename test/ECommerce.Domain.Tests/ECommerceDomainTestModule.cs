using Volo.Abp.Modularity;

namespace ECommerce;

[DependsOn(
    typeof(ECommerceDomainModule),
    typeof(ECommerceTestBaseModule)
)]
public class ECommerceDomainTestModule : AbpModule
{

}
