using Abp.Modules;
using Abp.Reflection.Extensions;

namespace MDM.EnvironmentModule
{
    [DependsOn()]
    public class EnvironmentModule : AbpModule
    {
        public override void PreInitialize()
        {

        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EnvironmentModule).GetAssembly());

        }

        public override void PostInitialize()
        {
            //IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}