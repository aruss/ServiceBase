// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Localization.Routing;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Options;

    public class UnsupportedCulturesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _routeDataStringKey;

        public UnsupportedCulturesMiddleware(
            RequestDelegate next,
            IOptions<RequestLocalizationOptions> options)
        {
            this._next = next;

            RouteDataRequestCultureProvider provider = options.Value.RequestCultureProviders
                .Select(x => x as RouteDataRequestCultureProvider)
                .Where(x => x != null)
                .FirstOrDefault();

            this._routeDataStringKey = provider.RouteDataStringKey;
        }

        public async Task Invoke(HttpContext context)
        {
            string requestedCulture = context.GetRouteValue(_routeDataStringKey)?.ToString();
            IRequestCultureFeature cultureFeature = context.Features.Get<IRequestCultureFeature>();
            string actualCulture = cultureFeature?.RequestCulture.Culture.Name;

            if (string.IsNullOrEmpty(requestedCulture) ||
                !string.Equals(requestedCulture, actualCulture, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 404;
                return;
            }

            await _next.Invoke(context);
        }
    }
}

