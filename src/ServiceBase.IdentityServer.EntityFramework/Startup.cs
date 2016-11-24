using Microsoft.Extensions.DependencyInjection;

namespace ServiceBase.IdentityServer.EntityFramework
{
    // Only for `dotnet ef migrations` command
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkStores(opt =>
            {
                opt.ConnectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=Test.IdentityServer4.EntityFramework;trusted_connection=yes;";

                opt.SeedExampleData = false;
            });
        }
    }
}
