namespace ServiceBase.ExtensionHost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class ServiceCollectionExtensions
    {
        public static void AddExtensions(
            this IServiceCollection services,
            string extensionsPath,
            bool includingSubpaths = true)
        {
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            AssemblyProvider assemblyProvider =
                new AssemblyProvider(serviceProvider);

            IEnumerable<Assembly> assemblies = assemblyProvider
                .GetAssemblies(extensionsPath, includingSubpaths);

            ExtensionManager.SetAssemblies(assemblies);

            ILogger logger = serviceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger("Extensions");

            foreach (IConfigureServicesAction action in ExtensionManager
                .GetServices<IConfigureServicesAction>())
            {
                logger.LogInformation(
                    "Executing ConfigureServices action '{0}'",
                    action.GetType().FullName);

                action.Execute(services, serviceProvider);
                serviceProvider = services.BuildServiceProvider();
            }
        }

        public static void AddMvcHost(
            this IServiceCollection services)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            ILogger logger = serviceProvider
                .GetService<ILoggerFactory>()
                .CreateLogger("Extensions");

            services
                .AddRouting((options) =>
                {
                    options.LowercaseUrls = true;
                });

            IMvcBuilder mvcBuilder = services
                .AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            foreach (Assembly assembly in ExtensionManager.Assemblies)
            {
                mvcBuilder.AddApplicationPart(assembly);
            }

            mvcBuilder.AddRazorOptions(razor =>
            {
                IEnumerable<MetadataReference> refs =
                    ExtensionManager.Assemblies
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
                    .Add(new CustomViewLocationExpander(
                        // TODO: pass request theme infor provider via options
                        new DefaultRequestThemeInfoProvider()));
            });

            foreach (IAddMvcAction action in ExtensionManager
                .GetServices<IAddMvcAction>())
            {
                logger.LogInformation(
                    "Executing AddMvc action '{0}'",
                    action.GetType().FullName);

                action.Execute(mvcBuilder, serviceProvider);
            }
        }
    }
}
