using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using HizliSu.Authorization;

namespace HizliSu
{
    [DependsOn(
        typeof(HizliSuCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class HizliSuApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<HizliSuAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(HizliSuApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
