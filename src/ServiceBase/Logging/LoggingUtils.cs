// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Logging
{
    using Microsoft.AspNetCore.Http;
    using Serilog;

    public static class LoggingUtils
    {
        public static void EnrichFromRequest(
            IDiagnosticContext diagnosticCtx,
            HttpContext httpCtx)
        {
            HttpRequest request = httpCtx.Request;

            // Set all the common properties available for every request
            diagnosticCtx.Set("Host", request.Host);
            diagnosticCtx.Set("Protocol", request.Protocol);
            diagnosticCtx.Set("Scheme", request.Scheme);

            diagnosticCtx.Set("RemoteIp",
                httpCtx.Request.HttpContext.Connection.RemoteIpAddress);

            // Only set it if available. You're not sending sensitive data
            // in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticCtx.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticCtx.Set("ContentType", httpCtx.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpCtx.GetEndpoint();
            if (endpoint is object) // endpoint != null
            {
                diagnosticCtx.Set("EndpointName", endpoint.DisplayName);
            }
        }
    }
}
