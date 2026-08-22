using Volo.Abp.GlobalFeatures;
using Volo.Abp.Threading;
using Volo.CmsKit.GlobalFeatures;

namespace ECommerce;

public static class ECommerceGlobalFeatureConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            /* Enable CmsKit (pages, blog, comments, ratings, tags, menus) */
            GlobalFeatureManager.Instance.Modules.CmsKit(cmsKit =>
            {
                cmsKit.EnableAll();
            });
        });
    }
}
