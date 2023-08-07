using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Identity.EntityFrameworkCore;
using MDM.CatalogService.Core;

namespace MDM.CatalogService.EntityFrameworkCore;
[DependsOn(
      typeof(CoreModule),
      typeof(MDMEntityFrameworkModule),
      typeof(AbpEntityFrameworkCoreModule))]
public class CatalogEntityFrameworkModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Modules.AbpEfCore().AddDbContext<CatalogDbContext>(options =>
        {
            if (options.ExistingConnection != null)
            {
                CatalogDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
            }
            else
            {
                CatalogDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
            }
        });
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(CatalogEntityFrameworkModule).GetAssembly());
    }

    public override void PostInitialize()
    {
    }
}