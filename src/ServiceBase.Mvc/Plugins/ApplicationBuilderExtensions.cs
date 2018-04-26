﻿namespace ServiceBase.Mvc.Plugins
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Mvc.Theming;

    public static class ApplicationBuilderExtensions
    {
        public static void UsePlugins(
            this IApplicationBuilder app)
        {
            ILogger logger = app.ApplicationServices
                .GetService<ILoggerFactory>()
                .CreateLogger("Extensions");

            foreach (IConfigureAction action in PluginManager
                .GetServices<IConfigureAction>())
            {
                logger.LogInformation(
                    "Executing Configure action '{0}'",
                    action.GetType().FullName);

                action.Execute(app, app.ApplicationServices);
            }
        }

        public static void UsePluginsMvcHost(
            this IApplicationBuilder app,
            string extensionsPath)
        {
            ILogger logger = app.ApplicationServices
                .GetService<ILoggerFactory>().CreateLogger("Extensions");

            app.UseMvc(routeBuilder =>
            {
                foreach (IUseMvcAction action in PluginManager
                    .GetServices<IUseMvcAction>())
                {
                    logger.LogInformation(
                        "Executing UseMvc action '{0}'",
                        action.GetType().FullName);

                    action.Execute(routeBuilder, app.ApplicationServices);
                }

                routeBuilder.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new ThemeFileProvider(extensionsPath)
            });
        }
    }
}
