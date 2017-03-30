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
            var builder = new ConfigurationBuilder()
               .SetBasePath(environment.ContentRootPath)
               .AddJsonFile(Path.Combine("Config", "config.json"), optional: false, reloadOnChange: true)
               .AddJsonFile(Path.Combine("Config", $"config.{environment.EnvironmentName}.json"), optional: true, reloadOnChange: true);
                        
            builder.AddEnvironmentVariables();

            configAction?.Invoke(builder);

            return builder.Build();
        }
    }
}
