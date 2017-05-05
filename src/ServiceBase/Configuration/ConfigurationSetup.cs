using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ServiceBase.Configuration
{
    public static class ConfigurationSetup
    {
        public static IConfigurationRoot Configure(IHostingEnvironment environment, Action<IConfigurationBuilder> configAction = null)
        {
            return Configure(environment.ContentRootPath, environment.EnvironmentName, configAction); 
        }

        public static IConfigurationRoot Configure(string contentRootPath, string environmentName, Action<IConfigurationBuilder> configAction = null)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(contentRootPath)
               .AddJsonFile(Path.Combine("Config", "config.json"), optional: false, reloadOnChange: true)
               .AddJsonFile(Path.Combine("Config", $"config.{environmentName}.json"), optional: true, reloadOnChange: true);
                        
            builder.AddEnvironmentVariables();

            configAction?.Invoke(builder);

            return builder.Build();
        }
    }
}
