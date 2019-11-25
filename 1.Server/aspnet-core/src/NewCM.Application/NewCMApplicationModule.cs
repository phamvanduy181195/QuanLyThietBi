using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using NewCM.Authorization;

namespace NewCM
{
    [DependsOn(typeof(NewCMCoreModule), typeof(AbpAutoMapperModule))]
    public class NewCMApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<NewCMAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(NewCMApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
