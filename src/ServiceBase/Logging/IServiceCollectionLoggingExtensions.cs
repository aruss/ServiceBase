// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Microsoft.Extensions.Logging
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyModel;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Serilog.Core;

    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods.
    /// </summary>
    public static partial class IServiceCollectionLoggingExtensions
    {
        /// <summary>
        /// Adds Serilog logging services
        /// </summary>
        /// <param name="services">Instance of
        /// <see cref="IServiceCollection"/></param>
        /// <param name="config">Instance of <see cref="IConfiguration"/>
        /// </param>
        /// <returns>Instance of
        /// <see cref="IServiceCollection"/> provided via
        /// <paramref name="services"/> argument</returns>
        public static IServiceCollection AddSerilog(
            this IServiceCollection services,
            IConfiguration config)
        {
            return services.AddLogging(builder =>
            {
                builder.AddSerilog(config);
            });
        }

        /// <summary>
        /// Adds Serilog logger
        /// </summary>
        /// <param name="loggingBuilder">Instance of
        /// <see cref="ILoggingBuilder"/></param>
        /// <param name="config">Instance of <see cref="IConfiguration"/>
        /// </param>
        /// <returns>Instance of
        /// <see cref="ILoggingBuilder"/> provided via
        /// <paramref name="loggingBuilder"/> argument</returns>
        public static ILoggingBuilder AddSerilog(
            this ILoggingBuilder loggingBuilder,
            IConfiguration config)
        {
            Logger logger = new LoggerConfiguration()
                .ReadFrom.Configuration(
                    config,
                    "Serilog",
                    DependencyContext.Default
                )
                .CreateLogger(); 
            
            return loggingBuilder.AddSerilog(logger, true);
        }
    }
}