using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration; 

namespace ServiceBase
{
    public static class ConfigurationSetup
    {
        public static IConfigurationRoot Configure(IHostingEnvironment hostEnv)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(hostEnv.ContentRootPath)
               .AddJsonFile(Path.Combine("Config", "config.json"), optional: false, reloadOnChange: true)
               .AddJsonFile(Path.Combine("Config", $"config.{hostEnv.EnvironmentName}.json"), optional: true, reloadOnChange: true);

            if (hostEnv.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
