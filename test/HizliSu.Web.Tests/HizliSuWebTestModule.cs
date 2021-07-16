using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using HizliSu.EntityFrameworkCore;
using HizliSu.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace HizliSu.Web.Tests
{
    [DependsOn(
        typeof(HizliSuWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class HizliSuWebTestModule : AbpModule
    {
        public HizliSuWebTestModule(HizliSuEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(HizliSuWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(HizliSuWebMvcModule).Assembly);
        }
    }
}