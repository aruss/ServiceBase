namespace ServiceBase.Plugins
{
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Mvc.Theming;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static partial class IServiceCollectionExtensions
    {
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
