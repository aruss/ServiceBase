namespace ServiceBase.Mvc.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Mvc.Theming;

    public static partial class IServiceCollectionExtensions
    {
        public static void AddPlugins(
            this IServiceCollection serviceCollection)
        {
            IServiceProvider serviceProvider =
                serviceCollection.BuildServiceProvider();

            ILogger logger = serviceProvider
                 .GetService<ILoggerFactory>()
                 .CreateLogger("Plugins");

            IEnumerable<IConfigureServicesAction> actions =
                PluginAssembyLoader.GetServices<IConfigureServicesAction>();

            foreach (IConfigureServicesAction action in actions)
            {
                logger.LogInformation(
                    "Executing ConfigureServices action '{0}'",
                    action.GetType().FullName);

                try
                {
                    action.Execute(serviceCollection);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Executing ConfigureServices action '{0}' caused an error.",
                        action.GetType().FullName
                    );
                }
            }
        }

        public static void AddPluginsMvcHost(
            this IServiceCollection services,
            string pluginsPath,
            IRequestThemeInfoProvider requestThemeInfoProvider = null)
        {
            ServiceProvider serviceProvider =
                services.BuildServiceProvider();

            ILogger logger = serviceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger("Plugins");

            services
                .AddRouting((options) =>
                {
                    options.LowercaseUrls = true;
                });

            IMvcBuilder mvcBuilder = services
                .AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            IEnumerable<IAddMvcAction> actions = PluginAssembyLoader
                .GetServices<IAddMvcAction>();

            IEnumerable<Assembly> assemblies = actions
                .Select(s => s.GetType().Assembly);

            foreach (Assembly assembly in assemblies)
            {
                mvcBuilder.AddApplicationPart(assembly);
            }

            mvcBuilder.AddRazorOptions(razor =>
            {
                IEnumerable<MetadataReference> refs = assemblies
                        .Where(x => !x.IsDynamic &&
                            !string.IsNullOrWhiteSpace(x.Location))
                        .Select(x => MetadataReference
                            .CreateFromFile(x.Location));

                foreach (var portableExecutableReference in refs)
                {
                    razor.AdditionalCompilationReferences
                        .Add(portableExecutableReference);
                }

                razor.ViewLocationExpanders.Clear();

                razor.ViewLocationExpanders
                    .Add(new ThemeViewLocationExpander(
                        pluginsPath,
                        requestThemeInfoProvider ??
                        new DefaultRequestThemeInfoProvider()));
            });

            foreach (IAddMvcAction action in actions)
            {
                logger.LogInformation(
                    $"Executing AddMvc action '{0}'",
                    action.GetType().FullName);

                try
                {
                    action.Execute(mvcBuilder);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        $"Executing AddMvc action '{0}' caused an error.",
                        action.GetType().FullName
                    );
                }
            }
        }
    }
}
