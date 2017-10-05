namespace ServiceBase
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Configuration utilities
    /// </summary>
    public static class ConfigUtils
    {
        /// <summary>
        /// Loads a configuration from file and extends it with user secrets,
        /// environment vars and command line args
        /// </summary>
        /// <typeparam name="TStartup">Type of the startup class</typeparam>
        /// <param name="args">Command line arguments, if not provided no
        /// command line will be added to config builder </param>
        /// <param name="path">Json config file path, if not provided
        /// "./AppData/config.json" will be taken.</param>
        /// <param name="basePath">Application base path, if not provided
        /// current directory path will be taken.</param>
        /// <returns>Instance of <see cref="IConfigurationRoot"/>.</returns>
        public static IConfiguration LoadConfiguration<TStartup>(
            string[] args = null,
            string path = null,
            string basePath = null)
            where TStartup : class
        {
            bool isDevelopment = Environment
                .GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                .Equals("Development", StringComparison.OrdinalIgnoreCase);

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath ?? Directory.GetCurrentDirectory())
                .AddJsonFile(path ?? "./AppData/config.json",
                                optional: false,
                                reloadOnChange: false);

            if (isDevelopment)
            {
                configBuilder.AddUserSecrets<TStartup>();
            }

            configBuilder.AddEnvironmentVariables();

            if (args != null)
            {
                configBuilder.AddCommandLine(args);
            }
            
            return configBuilder.Build();
        }
    }
}
