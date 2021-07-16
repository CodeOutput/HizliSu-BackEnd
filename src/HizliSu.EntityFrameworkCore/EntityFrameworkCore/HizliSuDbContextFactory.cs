using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using HizliSu.Configuration;
using HizliSu.Web;

namespace HizliSu.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class HizliSuDbContextFactory : IDesignTimeDbContextFactory<HizliSuDbContext>
    {
        public HizliSuDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<HizliSuDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            HizliSuDbContextConfigurer.Configure(builder, configuration.GetConnectionString(HizliSuConsts.ConnectionStringName));

            return new HizliSuDbContext(builder.Options);
        }
    }
}
