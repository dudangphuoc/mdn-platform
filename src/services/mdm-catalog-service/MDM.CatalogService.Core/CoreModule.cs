using Abp.Modules;
using Abp.Reflection.Extensions;
using MDMPlatform;

namespace MDM.CatalogService.Core;

[DependsOn(
    typeof(MDMPlatformCoreModule)
)]
public class CoreModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(CoreModule).GetAssembly());
    }
}