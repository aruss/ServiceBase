// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using ServiceBase.Extensions;

    public static class MvcApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMvcWithCultureRoute(
            this IApplicationBuilder app)
        {
            return MvcApplicationBuilderExtensions.UseMvcWithCultureRoute(app, null);
        }

        public static IApplicationBuilder UseMvcWithCultureRoute(
            this IApplicationBuilder app,
            Action<IRouteBuilder> configureRoutes)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "defaultWithCulture",
                    template: "{culture:culturecode}/{controller=Home}/{action=Index}/{id?}");

                configureRoutes?.Invoke(routes);
            });

            RequestLocalizationOptions localizationOptions = app
                   .ApplicationServices
                   .GetService<IOptions<RequestLocalizationOptions>>().Value;
            
            app.UseRouter(builder =>
            {
                builder.MapGet("{culture:culturecode}/{*path}", ctx =>
                {
                    return Task.CompletedTask;
                });

                builder.MapGet("{*path}", (RequestDelegate)(ctx =>
                {
                    string defaultCulture = localizationOptions
                        .DefaultRequestCulture.Culture.Name.ToLower();

                    object path = ctx.GetRouteValue("path") ?? string.Empty;
                    ctx.Response.Redirect($"/{defaultCulture}/{path}".RemoveTrailingSlash());
                    return Task.CompletedTask;
                }));
            });

            return app;
        }
    }
}