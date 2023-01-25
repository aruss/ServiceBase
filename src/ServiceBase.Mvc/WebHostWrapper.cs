// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase
{
    using System;
    using System.IO;
    using System.Threading;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyModel;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using ServiceBase.Extensions;

    public class WebHostWrapper
    {
        private static CancellationTokenSource
            cancelTokenSource = new CancellationTokenSource();

        private static string GetContentRoot()
        {
            string contentRoot = EnironmentUtils.GetContentRoot();

            if (!string.IsNullOrWhiteSpace(contentRoot))
            {
                FileAttributes attr = File.GetAttributes(contentRoot);

                if ((attr & FileAttributes.Directory) !=
                    FileAttributes.Directory)
                {
                    throw new ArgumentException(
                        $"Given Content root \"{contentRoot}\"is not a valid directory"
                    );
                }

                return contentRoot;
            }

            return Directory.GetCurrentDirectory();
        }

        public static int Start<TStartup>(
            string[] args,
            Func<IHostBuilder, IConfiguration, IHostBuilder> configureHostBuilder = null,
            Action<IWebHostBuilder, IConfiguration> configureWebHostBuilder = null)
            where TStartup : class
        {
            string contentRoot = WebHostWrapper.GetContentRoot();

            IConfiguration config =
                ConfigUtils.LoadConfig<TStartup>(args, contentRoot);

            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(
                   config,
                   "Serilog",
                   DependencyContext.Default
               )
               .Enrich.FromLogContext()
               .CreateLogger();

            try
            {
                WebHostWrapper.Start<TStartup>(
                    contentRoot,
                    config,
                    args,
                    configureHostBuilder,
                    configureWebHostBuilder);

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void Start<TStartup>(
            string contentRoot,
            IConfiguration config,
            string[] args,
            Func<IHostBuilder, IConfiguration, IHostBuilder> configureHostBuilder = null,
            Action<IWebHostBuilder, IConfiguration> configureWebHostBuilder = null)
            where TStartup : class
        {
            Log.Information("Starting web host");

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(config =>
                {
                    config.ClearProviders();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    string urls = config["Host:Urls"];

                    webBuilder
                        .ConfigureAppConfiguration((context, config) =>
                        {
                            foreach (var s in config.Sources)
                            {
                                if (s is FileConfigurationSource)
                                    ((FileConfigurationSource)s).ReloadOnChange = false;
                            }
                        })
                        .UseStartup<TStartup>()
                        .UseContentRoot(contentRoot)
                        .UseConfiguration(config)
                        .UseUrls(urls.IsPresent() ? urls : "http://*:8080");

                    configureWebHostBuilder?.Invoke(webBuilder, config);
                })
                .UseSerilog();

            if (configureHostBuilder != null)
            {
                hostBuilder = configureHostBuilder.Invoke(hostBuilder, config);
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
    }
}
