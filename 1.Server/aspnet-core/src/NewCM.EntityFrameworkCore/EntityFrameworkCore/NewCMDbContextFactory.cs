using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NewCM.Configuration;
using NewCM.Web;

namespace NewCM.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class NewCMDbContextFactory : IDesignTimeDbContextFactory<NewCMDbContext>
    {
        public NewCMDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<NewCMDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            NewCMDbContextConfigurer.Configure(builder, configuration.GetConnectionString(NewCMConsts.ConnectionStringName));

            return new NewCMDbContext(builder.Options);
        }
    }
}
