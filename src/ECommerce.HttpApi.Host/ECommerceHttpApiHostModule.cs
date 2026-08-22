using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Server.AspNetCore;
using ECommerce.EntityFrameworkCore;
using ECommerce.MultiTenancy;
using ECommerce.HealthChecks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using Volo.Abp;
using Volo.Abp.Studio;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using ECommerce.Catalog;
using ECommerce.Hubs;
using ECommerce.Notifications;
using ECommerce.OrganizationSignup;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.Security.Claims;

namespace ECommerce;

[DependsOn(
    typeof(ECommerceHttpApiModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(ECommerceApplicationModule),
    typeof(ECommerceEntityFrameworkCoreModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule)
    )]
public class ECommerceHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("ECommerce");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", configuration["AuthServer:CertificatePassPhrase"]!);
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        }

        if (!configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata"))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            });
            
            Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        ConfigureStudio(hostingEnvironment);
        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureBundles(hostingEnvironment);
        ConfigureConventionalControllers();
        ConfigureHealthChecks(context);
        ConfigureSwagger(context, configuration);
        ConfigureVirtualFileSystem(context);
        ConfigureCors(context, configuration);
        if (MultiTenancyConsts.IsEnabled)
        {
            ConfigureMultiTenancy(configuration);
        }

        context.Services.AddTransient<IProductMediaFileStorage, ProductMediaFileStorage>();
        context.Services.AddTransient<IOrganizationSignupLogoStorage, OrganizationSignupLogoFileStorage>();

        context.Services.AddSignalR();
        context.Services.AddTransient<INotificationPublisherService, NotificationPublisherService>();
    }

    private void ConfigureMultiTenancy(IConfiguration configuration)
    {
        Configure<AbpAspNetCoreMultiTenancyOptions>(options =>
        {
            var tenantKey = configuration["MultiTenancy:TenantKey"];
            if (!string.IsNullOrWhiteSpace(tenantKey))
            {
                options.TenantKey = tenantKey;
            }
        });

        Configure<AbpTenantResolveOptions>(options =>
        {
            /* AbpAspNetCoreMultiTenancyModule already registers QueryString, Route, Header, Cookie.
             * Append subdomain resolution so production can use {tenant}.domain while dev uses ?__tenant= or header. */
            var domainFormat = configuration["MultiTenancy:DomainFormat"];
            if (!string.IsNullOrWhiteSpace(domainFormat))
            {
                options.TenantResolvers.Add(new DomainTenantResolveContributor(domainFormat));
            }
        });
    }

    private void ConfigureStudio(IHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsProduction())
        {
            Configure<AbpStudioClientOptions>(options =>
            {
                options.IsLinkEnabled = false;
            });
        }
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.Applications["Angular"].RootUrl = configuration["App:AngularUrl"];
            options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());
        });
    }

    private void ConfigureBundles(IHostEnvironment hostingEnvironment)
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );

            options.ScriptBundles.Configure(
                LeptonXLiteThemeBundles.Scripts.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-scripts.js");
                    if (hostingEnvironment.IsDevelopment())
                    {
                        bundle.AddFiles("/dev-login-helper.js");
                    }
                }
            );
        });
    }


    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<ECommerceDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}ECommerce.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<ECommerceDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}ECommerce.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<ECommerceApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}ECommerce.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<ECommerceApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}ECommerce.Application"));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(ECommerceApplicationModule).Assembly, conventionalControllerOptions =>
            {
                // Exclude app services exposed via custom controllers (route "tree"/"list"/"attributes" before {id}).
                conventionalControllerOptions.TypePredicate = type =>
                    type != typeof(ECommerce.Catalog.ProductMediaAppService) &&
                    type != typeof(ECommerce.Catalog.CategoryAppService) &&
                    type != typeof(ECommerce.Catalog.ProductAppService) &&
                    type != typeof(ECommerce.Catalog.PublicCatalogAppService);
            });
        });
    }

    private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOidc(
            configuration["AuthServer:Authority"]!,
            ["ECommerce"],
            [AbpSwaggerOidcFlows.AuthorizationCode],
            null,
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
                // Prefer explicit controller actions over auto-generated app service endpoints when path conflicts (e.g. analytics).
                options.ResolveConflictingActions(apiDescriptions =>
                {
                    var fromController = apiDescriptions.FirstOrDefault(a =>
                        a.ActionDescriptor is ControllerActionDescriptor cad &&
                        cad.ControllerTypeInfo.Assembly.GetName().Name == "ECommerce.HttpApi");
                    return fromController ?? apiDescriptions.First();
                });
            });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private void ConfigureHealthChecks(ServiceConfigurationContext context)
    {
        context.Services.AddECommerceHealthChecks();
    }


    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Switching UI language can trigger browser-initiated request cancellation during reload/navigation.
        // Ignore the known transport IOException for aborted requests to avoid noisy/unhandled errors.
        app.Use(async (httpContext, next) =>
        {
            try
            {
                await next();
            }
            catch (OperationCanceledException) when (httpContext.RequestAborted.IsCancellationRequested)
            {
                return;
            }
            catch (IOException ex) when (
                httpContext.RequestAborted.IsCancellationRequested ||
                ex.Message.Contains("The client reset the request stream.", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        });

        app.UseAbpRequestLocalization();

        // Redirect backend Account/Register to Angular full customer subscription form (contact + addresses).
        app.Use(async (httpContext, next) =>
        {
            var path = httpContext.Request.Path.Value ?? "";
            if (httpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase) &&
                path.Equals("/Account/Register", StringComparison.OrdinalIgnoreCase))
            {
                var config = httpContext.RequestServices.GetRequiredService<IConfiguration>();
                var angularUrl = (config["App:AngularUrl"] ?? "http://localhost:4200").TrimEnd('/');
                var query = httpContext.Request.QueryString.HasValue ? httpContext.Request.QueryString.Value : "";
                var redirectUrl = query.Length > 0 ? $"{angularUrl}/account/register{query}" : $"{angularUrl}/account/register";
                httpContext.Response.Redirect(redirectUrl);
                return;
            }
            await next();
        });

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseRouting();
        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();
        app.UseAbpSecurityHeaders();
        app.UseCors();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API");

            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapHub<NotificationHub>("/hubs/notification");
        });
    }
}
