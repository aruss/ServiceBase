// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Logging
{
    using System.Net;
    using Microsoft.AspNetCore.Builder;

    public static partial class IApplicationBuilderExtensions
    {
        /// <summary>
        /// Extens serilog logs with additional information like remote IP
        /// address
        /// </summary>
        /// <param name="app">Instance of <see cref="IApplicationBuilder"/>.
        /// </param>
        public static void UseSerilog(this IApplicationBuilder app)
        {
            app.Use(async (ctx, next) =>
            {
                IPAddress remoteIpAddress = ctx.Request
                    .HttpContext.Connection.RemoteIpAddress;

                using (Serilog.Context.LogContext
                    .PushProperty("RemoteIpAddress", remoteIpAddress))
                {
                    await next();
                }
            });
        }
    }
}
