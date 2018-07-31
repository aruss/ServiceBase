// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// Contains <see cref="IApplicationBuilder"/> extenion methods.
    /// </summary>
    public static partial class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds services of each plugins to the
        /// <see cref="IApplicationBuilder"/> request execution pipeline.
        /// </summary>
        public static void UsePlugins(
            this IApplicationBuilder app)
        {
            IEnumerable<IConfigureAction> actions =
                PluginAssembyLoader.GetServices<IConfigureAction>();

            foreach (IConfigureAction action in actions)
            {
                action.Execute(app);
            }
        }
    }
}
