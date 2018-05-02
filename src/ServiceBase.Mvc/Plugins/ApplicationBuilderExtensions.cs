namespace ServiceBase.Mvc.Plugins
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Mvc.Theming;

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

        /// <summary>
        /// Adds MVC to the <see cref="IApplicationBuilder"/> request
        /// execution pipeline, configured to work with plugin architecture.
        /// </summary>
        /// <param name="app">
        /// Instance of <see cref="IApplicationBuilder"/>.
        /// </param>
        public static void UsePluginsMvc(
                this IApplicationBuilder app)
        {
            ILogger logger = app.ApplicationServices
                .GetService<ILoggerFactory>().CreateLogger("Plugins");

            app.UseMvc(routeBuilder =>
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
        }

        /// <summary>
        ///  Enables static file serving with plugin architecture.
        /// </summary>
        /// <param name="app">
        /// Instance of <see cref="IApplicationBuilder"/>.
        /// </param>
        /// <param name="basePath">
        /// Base path to plugins folder.
        /// </param>
        public static void UsePluginsStaticFiles(
                this IApplicationBuilder app,
                string basePath)
        {
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PluginsFileProvider(basePath)
            });
        }
    }
}
