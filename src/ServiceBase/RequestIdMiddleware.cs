// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class RequestIdMiddleware
    {
        private const string headerName = "X-Request-ID";
        private readonly RequestDelegate _next;

        public RequestIdMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!String.IsNullOrWhiteSpace(
                context.Request.Headers[headerName]))
            {
                context.TraceIdentifier = context.Request.Headers[headerName];
            }

            context.Response.Headers[headerName] = context.TraceIdentifier;

            await this._next(context);
        }
    }
}
