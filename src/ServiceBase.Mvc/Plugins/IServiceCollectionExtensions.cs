// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Mvc.Razor.Compilation;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static partial class IServiceCollectionExtensions
    {
        public static void AddPluginsMvc(
            this IServiceCollection services,
            ILogger logger)
        {
            var mvcBuilder = services.AddMvc()
                /*.ConfigureApplicationPartManager(manager =>
                {
                    IApplicationFeatureProvider toRemove = manager
                        .FeatureProviders
                        .First(f => f is MetadataReferenceFeatureProvider);

                    manager.FeatureProviders
                        .Remove(toRemove);

                    manager.FeatureProviders
                        .Add(new ReferencesMetadataReferenceFeatureProvider());
                })*/;

            IEnumerable<Assembly> assemblies = PluginAssembyLoader.Assemblies;
            foreach (Assembly assembly in assemblies)
            {
                logger.LogDebug(
                    "Adding mvc application part: \"{0}\"",
                    assembly.FullName
                );

                mvcBuilder.AddApplicationPart(assembly);
            }

            IEnumerable<IAddMvcAction> actions = PluginAssembyLoader
                .GetServices<IAddMvcAction>();

            foreach (IAddMvcAction action in actions)
            {
                logger.LogDebug(
                    "Executing add mvc action \"{0}\"",
                    action.GetType().FullName
                );

                action.Execute(mvcBuilder);
            }
        }
    }
}
