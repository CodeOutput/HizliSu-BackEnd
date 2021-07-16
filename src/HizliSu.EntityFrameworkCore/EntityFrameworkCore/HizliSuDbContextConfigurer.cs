using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace HizliSu.EntityFrameworkCore
{
    public static class HizliSuDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<HizliSuDbContext> builder, string connectionString)
        {
            // builder.UseSqlServer(connectionString);
            builder.UseNpgsql(connectionString);

        }

        public static void Configure(DbContextOptionsBuilder<HizliSuDbContext> builder, DbConnection connection)
        {
            // builder.UseSqlServer(connection);
            builder.UseNpgsql(connection);
        }
    }
}
