using ECommerce.Localization;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization.Permissions;

namespace ECommerce;

/* Inherit your application services from this class.
 */
public abstract class ECommerceAppService : ApplicationService
{
    /// <summary>ABP does not expose this on <see cref="ApplicationService"/> in all versions; use for permission checks in app services.</summary>
    protected IPermissionChecker PermissionChecker => LazyServiceProvider.LazyGetRequiredService<IPermissionChecker>();

    protected ECommerceAppService()
    {
        LocalizationResource = typeof(ECommerceResource);
    }
}
