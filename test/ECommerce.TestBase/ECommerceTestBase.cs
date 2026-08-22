using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Testing;

namespace ECommerce;

public abstract class ECommerceTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }

    protected override void BeforeAddApplication(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", false);
        builder.AddJsonFile("appsettings.secrets.json", true);
        services.ReplaceConfiguration(builder.Build());
    }

    protected virtual Task WithUnitOfWorkAsync(Func<Task> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task WithUnitOfWorkAsync(AbpUnitOfWorkOptions options, Func<Task> action)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin(options))
            {
                await action();

                await uow.CompleteAsync();
            }
        }
    }

    protected virtual Task<TResult> WithUnitOfWorkAsync<TResult>(Func<Task<TResult>> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task<TResult> WithUnitOfWorkAsync<TResult>(AbpUnitOfWorkOptions options, Func<Task<TResult>> func)
    {
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            using (var uow = uowManager.Begin(options))
            {
                var result = await func();
                await uow.CompleteAsync();
                return result;
            }
        }
    }

    /// <summary>
    /// Runs <paramref name="action"/> under the default integration-test tenant so IMultiTenant queries and inserts match seeded data.
    /// </summary>
    protected virtual async Task WithDefaultTestTenantAsync(Func<Task> action)
    {
        var currentTenant = GetRequiredService<ICurrentTenant>();
        using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
        {
            await action();
        }
    }

    /// <summary>
    /// Runs <paramref name="func"/> under the default integration-test tenant so IMultiTenant queries and inserts match seeded data.
    /// </summary>
    protected virtual async Task<TResult> WithDefaultTestTenantAsync<TResult>(Func<Task<TResult>> func)
    {
        var currentTenant = GetRequiredService<ICurrentTenant>();
        using (currentTenant.Change(ECommerceTestConsts.DefaultTenantId))
        {
            return await func();
        }
    }
}
