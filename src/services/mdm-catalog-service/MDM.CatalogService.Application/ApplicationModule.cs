using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MDM.CatalogService.Core;

namespace MDM.CatalogService.Application;

[DependsOn(
    typeof(CoreModule),
    typeof(AbpAutoMapperModule))]
public class ApplicationModule : AbpModule
{
    public override void Initialize()
    {
        var thisAssembly = typeof(ApplicationModule).GetAssembly();
        IocManager.RegisterAssemblyByConvention(thisAssembly);
        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}