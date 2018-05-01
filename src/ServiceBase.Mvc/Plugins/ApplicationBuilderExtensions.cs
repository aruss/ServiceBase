namespace ServiceBase.Mvc.Plugins
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Mvc.Theming;

    public static partial class ApplicationBuilderExtensions
    {
        public static void UsePlugins(
            this IApplicationBuilder applicationBuilder)
        {
            IEnumerable<IConfigureAction> actions =
                PluginAssembyLoader.GetServices<IConfigureAction>();

            foreach (IConfigureAction action in actions)
            {
                action.Execute(applicationBuilder);
            }
        }

        public static void UsePluginsMvcHost(
                this IApplicationBuilder applicationBuilder,
                string pluginsPath)
        {
            ILogger logger = applicationBuilder.ApplicationServices
                .GetService<ILoggerFactory>().CreateLogger("Plugins");

            applicationBuilder.UseMvc(routeBuilder =>
            {
                foreach (IUseMvcAction action in PluginAssembyLoader
                    .GetServices<IUseMvcAction>())
                {
                    logger.LogInformation(
                        "Executing UseMvc action '{0}'",
                        action.GetType().FullName);

                    action.Execute(routeBuilder);
                }

                routeBuilder.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            applicationBuilder.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new ThemeFileProvider(pluginsPath)
            });
        }
    }

}
