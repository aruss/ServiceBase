// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Contains <see cref="IApplicationBuilder"/> extenion methods.
    /// </summary>
    public static partial class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds MVC to the <see cref="IApplicationBuilder"/> request
        /// execution pipeline, configured to work with plugin architecture.
        /// </summary>
        /// <param name="app">Instance of <see cref="IApplicationBuilder"/>.</param>
        /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
        public static void UsePluginsMvc(
            this IApplicationBuilder app,
            ILogger logger = null)
        {
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.ConfigurePlugins(logger);
            });
        }

        /// <summary>
        ///  Enables static file serving with plugin architecture.
        /// </summary>
        /// <param name="app">
        /// Instance of <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <param name="basePath">
        /// Base path to plugins folder.
        /// </param>
        public static void UsePluginsStaticFiles(
                this IApplicationBuilder app,
                string basePath)
        {
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PluginsFileProvider(basePath)
            });
        }
    }
}

