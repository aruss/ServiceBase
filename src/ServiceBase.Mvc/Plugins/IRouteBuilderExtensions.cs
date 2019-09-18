// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;

    public static class IRouteBuilderExtensions
    {
        /// <summary>
        /// Executes 
        /// </summary>
        /// <param name="routeBuilder">Instance of <see cref="IRouteBuilder"/>.</param>
        /// <param name="logger">Instance of <see cref="ILogger"/>.</param>
        public static void ConfigurePlugins(
            this IRouteBuilder routeBuilder,
            ILogger logger = null)
        {
            foreach (IUseMvcAction action in PluginAssembyLoader
                   .GetServices<IUseMvcAction>())
            {
                logger?.LogInformation(
                    "Executing UseMvc action '{0}'",
                    action.GetType().FullName);

                action.Execute(routeBuilder);
            }
        }
    }
}

