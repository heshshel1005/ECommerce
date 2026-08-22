using ECommerce.Email;
using ECommerce.Marketing;
using ECommerce.Payment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.EventBus;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.CmsKit;

namespace ECommerce;

[DependsOn(
    typeof(AbpEventBusModule),
    typeof(CmsKitApplicationModule),
    typeof(ECommerceDomainModule),
    typeof(ECommerceApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class ECommerceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.Configure<PaymobPaymentGatewayOptions>(configuration.GetSection(PaymobPaymentGatewayOptions.SectionName));
        context.Services.Configure<PayPalPaymentGatewayOptions>(configuration.GetSection(PayPalPaymentGatewayOptions.SectionName));
        context.Services.AddHttpClient();
        context.Services.AddTransient<IPaymentGateway, PaymobPaymentGateway>();
        context.Services.AddTransient<IPaymentGateway, PayPalPaymentGateway>();
        context.Services.AddTransient<IPaymentGateway, CashOnDeliveryPaymentGateway>();
        context.Services.AddTransient<ITransactionalEmailService, TransactionalEmailService>();
        context.Services.AddTransient<ILoyaltyPointsService, LoyaltyPointsService>();
    }
}
