// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Contains <see cref="IServiceCollection"/> extenion methods.
    /// </summary>
    public static partial class IServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="logger"></param>
        public static void AddPlugins(
            this IServiceCollection serviceCollection,
            ILogger logger)
        {
            IEnumerable<IConfigureServicesAction> actions =
                PluginAssembyLoader.GetServices<IConfigureServicesAction>();

            foreach (IConfigureServicesAction action in actions)
            {
                logger.LogInformation(
                    "Executing IConfigureServicesAction action \"{0}\"",
                    action.GetType().FullName);

                action.Execute(serviceCollection, logger);
            }
        }
    }
}
