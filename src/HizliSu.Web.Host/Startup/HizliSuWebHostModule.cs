using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using HizliSu.Configuration;

namespace HizliSu.Web.Host.Startup
{
    [DependsOn(
       typeof(HizliSuWebCoreModule))]
    public class HizliSuWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public HizliSuWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(HizliSuWebHostModule).GetAssembly());
        }
    }
}
