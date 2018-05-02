namespace ServiceBase.Mvc.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.Razor;
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
                 .CreateLogger(typeof(IServiceCollectionExtensions));

            IEnumerable<IConfigureServicesAction> actions =
                PluginAssembyLoader.GetServices<IConfigureServicesAction>();

            foreach (IConfigureServicesAction action in actions)
            {
                logger.LogInformation(
                    "Executing ConfigureServices action '{0}'",
                    action.GetType().FullName);

                action.Execute(serviceCollection);
            }
        }

        public static void AddPluginsMvc(
            this IServiceCollection services,
            string viewsBasePath)
        {
            services.AddPluginsMvc(
                new CustomViewLocationExpander(viewsBasePath));
        }

        public static void AddPluginsMvc(
            this IServiceCollection services,
            IThemeInfoProvider themeInfoProvider)
        {
            services.AddPluginsMvc(
                new ThemeViewLocationExpander(themeInfoProvider));
        }

        public static void AddPluginsMvc(
            this IServiceCollection services,
            IViewLocationExpander viewLocationExpander = null)
        {
            ServiceProvider serviceProvider =
                services.BuildServiceProvider();

            ILogger logger = serviceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger(typeof(IServiceCollectionExtensions));

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

                if (viewLocationExpander != null)
                {
                    razor.ViewLocationExpanders.Clear();
                    razor.ViewLocationExpanders.Add(viewLocationExpander);
                }
            });

            foreach (IAddMvcAction action in actions)
            {
                logger.LogInformation(
                    $"Executing AddMvc action '{0}'",
                    action.GetType().FullName);

                action.Execute(mvcBuilder);
            }
        }
    }
}
