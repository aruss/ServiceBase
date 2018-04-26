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

    public static class ServiceCollectionExtensions
    {
        public static void AddPlugins(
            this IServiceCollection services,
            string pluginsPath)
        {
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            ILogger logger = serviceProvider
               .GetService<ILoggerFactory>()
               .CreateLogger("Plugins");

            AssemblyProvider assemblyProvider = new AssemblyProvider(logger);

            IEnumerable<Assembly> assemblies = assemblyProvider
                .GetAssemblies(pluginsPath);

            PluginManager.SetAssemblies(assemblies);

            foreach (IConfigureServicesAction action in PluginManager
                .GetServices<IConfigureServicesAction>())
            {
                logger.LogInformation(
                    "Executing ConfigureServices action '{0}'",
                    action.GetType().FullName);

                try
                {
                    action.Execute(services, serviceProvider);
                    serviceProvider = services.BuildServiceProvider();
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        $"Executing ConfigureServices action '{action.GetType().FullName}' caused an error."
                    );
                }
            }
        }

        public static void AddPluginsMvcHost(
            this IServiceCollection services,
            string pluginsPath)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();

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

            foreach (Assembly assembly in PluginManager.Assemblies)
            {
                mvcBuilder.AddApplicationPart(assembly);
            }

            mvcBuilder.AddRazorOptions(razor =>
            {
                IEnumerable<MetadataReference> refs =
                    PluginManager.Assemblies
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
                        new DefaultRequestThemeInfoProvider()));
            });

            foreach (IAddMvcAction action in PluginManager
                .GetServices<IAddMvcAction>())
            {
                logger.LogInformation(
                    $"Executing AddMvc action '{action.GetType().FullName}'");

                try
                {
                    action.Execute(mvcBuilder, serviceProvider);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        $"Executing AddMvc action '{action.GetType().FullName}' caused an error."
                    );
                    throw;
                }                
            }
        }
    }
}
