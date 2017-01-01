using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ServiceBase.Configuration
{
    public static class ConfigurationSetup
    {
        public static IConfigurationRoot Configure(IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(environment.ContentRootPath)
               .AddJsonFile(Path.Combine("Config", "config.json"), optional: false, reloadOnChange: true)
               .AddJsonFile(Path.Combine("Config", $"config.{environment.EnvironmentName}.json"), optional: true, reloadOnChange: true);

            if (environment.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
