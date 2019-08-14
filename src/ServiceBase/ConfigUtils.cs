// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
        public static string GetConfigFilePath(string basePath)
        {
            string configRoot = EnironmentUtils.GetConfigRoot();

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

        public static IConfigurationRoot LoadConfig<TStartup>(
            string[] args,
            string basePath)
            where TStartup : class
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(
                    path: ConfigUtils.GetConfigFilePath(basePath),
                    optional: false,
                    reloadOnChange: false);

            if (EnironmentUtils.IsDevelopmentEnvironment())
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