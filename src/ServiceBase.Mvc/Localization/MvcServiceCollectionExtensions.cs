// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    public static class MvcServiceCollectionExtensions
    {
        public static IMvcBuilder AddMvcWithRouteDataRequestLocalization(
            this IServiceCollection services)
        {
            return MvcServiceCollectionExtensions
                .AddMvcWithLocalization(services, null);
        }

        public static IMvcBuilder AddMvcWithLocalization(
            this IServiceCollection services,
            Action<MvcOptions> setupAction)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap
                    .Add("culturecode", typeof(CultureRouteConstraint));

                options.LowercaseUrls = true;
            });

            return services.AddMvc(mvcOptions =>
            {
                setupAction?.Invoke(mvcOptions);
            })
               .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
               .AddViewLocalization()
               .AddDataAnnotationsLocalization();
        }
    }
}
