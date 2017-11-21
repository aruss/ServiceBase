// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Modules
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Represents a configurable module containing multiple servies.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Configurates the module services.
        /// </summary>
        /// <param name="services">
        /// Instance of <see cref="IServiceCollection"/>.
        /// </param>
        /// <returns>
        /// Instance of <see cref="IServiceProvider"/>.
        /// </returns>
        void ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration);

        /// <summary>
        /// Configures module services.
        /// </summary>
        /// <param name="app">
        /// Instance of <see cref="IApplicationBuilder"/>.
        /// </param>
        void Configure(IApplicationBuilder app);
    }
}
