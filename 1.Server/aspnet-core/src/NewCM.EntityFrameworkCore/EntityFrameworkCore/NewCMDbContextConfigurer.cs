using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace NewCM.EntityFrameworkCore
{
    public static class NewCMDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<NewCMDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<NewCMDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
