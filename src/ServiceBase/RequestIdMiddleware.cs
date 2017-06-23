using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServiceBase
{
    public class RequestIdMiddleware
    {
        private const string headerName = "X-Request-ID";
        private readonly RequestDelegate next;

        public RequestIdMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!String.IsNullOrWhiteSpace(context.Request.Headers[headerName]))
            {
                context.TraceIdentifier = context.Request.Headers[headerName];
            }

            context.Response.Headers[headerName] = context.TraceIdentifier;

            await this.next(context);
        }
    }
}
