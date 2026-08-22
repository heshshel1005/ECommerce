using Volo.Abp.Modularity;

namespace ECommerce;

/* Inherit from this class for your domain layer tests. */
public abstract class ECommerceDomainTestBase<TStartupModule> : ECommerceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
