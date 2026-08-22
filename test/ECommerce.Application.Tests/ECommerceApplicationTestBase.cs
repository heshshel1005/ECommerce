using Volo.Abp.Modularity;

namespace ECommerce;

public abstract class ECommerceApplicationTestBase<TStartupModule> : ECommerceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
