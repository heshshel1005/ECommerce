using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace ECommerce;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpBackgroundJobsAbstractionsModule)
)]
public class ECommerceTestBaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        context.Services.AddAlwaysAllowAuthorization();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => SeedTestDataAsync(context));
    }

    private static async Task SeedTestDataAsync(ApplicationInitializationContext context)
    {
        using var scope = context.ServiceProvider.CreateScope();
        var provider = scope.ServiceProvider;
        var dataSeeder = provider.GetRequiredService<IDataSeeder>();

        await dataSeeder.SeedAsync();

        if (!provider.GetRequiredService<IOptions<AbpMultiTenancyOptions>>().Value.IsEnabled)
        {
            return;
        }

        // Omitting the second IDataSeeder pass for DefaultTenantId: it hits SQLite UNIQUE on AbpPermissionGrants
        // with the current host + tenant seed pipeline. Integration tests use ICurrentTenant.Change(DefaultTenantId).
    }
}
