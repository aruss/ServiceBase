namespace ServiceBase
{
    using System;
    using System.IO;
    using System.Threading;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class WebHostWrapper
    {
        private static CancellationTokenSource
              cancelTokenSource = new CancellationTokenSource();

        public static void Start<TStartup>(
            string[] args,
            Action<IServiceCollection> configureServices = null)
             where TStartup : class
        {
            string contentRoot = Environment
                .GetEnvironmentVariable("ASPNETCORE_CONTENTROOT");

            if (!string.IsNullOrWhiteSpace(contentRoot))
            {
                FileAttributes attr = File.GetAttributes(contentRoot);

                if ((attr & FileAttributes.Directory) !=
                    FileAttributes.Directory)
                {
                    throw new ArgumentException(
                        $"Given Content root \"{contentRoot}\"is not a valid directory");
                }
            }
            else
            {
                contentRoot = Directory.GetCurrentDirectory();
            }

            WebHostWrapper.Start<TStartup>(
                args,
                contentRoot,
                configureServices);
        }

        /// <summary>
        /// Use this if you start identitybase from custom project 
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="args"></param>
        /// <param name="basePath"></param>
        public static void Start<TStartup>(
            string[] args,
            string basePath,
            Action<IServiceCollection> configureServices = null)
            where TStartup : class
        {
            IConfiguration config = WebHostWrapper
                .LoadConfig<TStartup>(args, basePath);

            // Use in case you changed the example data in ExampleData.cs file
            // Configuration.ExampleDataWriter.Write(config); 

            IConfigurationSection configHost = config.GetSection("Host");

            IWebHostBuilder hostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(configHost.GetValue<string>("Urls"))
                .UseContentRoot(basePath)
                .UseConfiguration(config)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddSerilog(hostingContext.Configuration);
                })
                .UseStartup<TStartup>();

            if (configureServices != null)
            {
                hostBuilder = hostBuilder
                    .ConfigureServices(configureServices);
            }

            if (configHost.GetValue<bool>("UseIISIntegration"))
            {
                Console.WriteLine("Enabling IIS Integration");
                hostBuilder = hostBuilder.UseIISIntegration();
            }

            hostBuilder
                .Build()
                .RunAsync(WebHostWrapper.cancelTokenSource.Token)
                .Wait();
        }

        public static void Restart()
        {
            WebHostWrapper.Shutdown(2);
        }

        public static void Shutdown(int exitCode = 0)
        {
            WebHostWrapper.cancelTokenSource.Cancel();
            Environment.ExitCode = exitCode;
        }

        private static string GetConfigFilePath(string basePath)
        {
            string configRoot = Environment
                .GetEnvironmentVariable("ASPNETCORE_CONFIGROOT");

            if (string.IsNullOrWhiteSpace(configRoot))
            {
                configRoot = Path.Combine(basePath, "config"); 
            }

            string configFilePath = Path.Combine(configRoot, "config.json");

            if (!File.Exists(Path.Combine(basePath, configFilePath)))
            {
                throw new ApplicationException(
                    $"Config file does not exists \"{configFilePath}\""); 
            }

            return configFilePath;
        }
                                                                                                           
        private static IConfigurationRoot LoadConfig<TStartup>(
            string[] args,
            string basePath)
            where TStartup : class
        {
            bool isDevelopment = WebHostWrapper.IsDevelopment();

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(
                    path: WebHostWrapper.GetConfigFilePath(
                        basePath
                    ),
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

        private static bool IsDevelopment()
        {
            return "Development".Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                StringComparison.OrdinalIgnoreCase
            );
        }
    }
}