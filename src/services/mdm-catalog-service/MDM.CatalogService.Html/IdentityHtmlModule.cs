using Abp.AspNetCore;
using Abp.AspNetCore.SignalR;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using MDM.CatalogService.Application;
using MDM.CatalogService.EntityFrameworkCore;
using MDM.CatalogService.Html.Authentication.JwtBearer;
using MDM.Shared.AuthorizationModule;
using MDMPlatform.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MDM.CatalogService.Html
{
    [DependsOn(
       typeof(ApplicationModule),
       typeof(CatalogEntityFrameworkModule),
       typeof(AbpAspNetCoreModule),
      typeof(AbpAspNetCoreSignalRModule)
   )]
    public class IdentityHtmlModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public IdentityHtmlModule(IWebHostEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
            _env = env;
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                GlobalConsts.ConnectionStringName
            );

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            //Configuration.Modules.AbpAspNetCore()
            //     .CreateControllersForAppServices(
            //         typeof(IdentityApplicationModule).GetAssembly()
            //     );

            ConfigureTokenAuth();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(IdentityHtmlModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(IdentityHtmlModule).Assembly);
        }

    }
}
