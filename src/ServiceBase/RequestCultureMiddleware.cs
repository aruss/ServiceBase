namespace ServiceBase
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(
            this IApplicationBuilder builder)
        {
            return builder.UseRequestCulture(new RequestCultureOptions());
        }

        public static IApplicationBuilder UseRequestCulture(
            this IApplicationBuilder builder, RequestCultureOptions options)
        {
            return builder.UseMiddleware<RequestCultureMiddleware>(options);
        }
    }

    public class RequestCultureOptions
    {
        public CultureInfo DefaultCulture { get; set; }
    }

    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate next;
        private readonly RequestCultureOptions options;

        public RequestCultureMiddleware(
            RequestDelegate next, RequestCultureOptions options)
        {
            this.next = next;
            this.options = options;
        }

        public Task Invoke(HttpContext context)
        {
            CultureInfo requestCulture = null;

            // context.Request.GetTypedHeaders().AcceptLanguage

            var cultureQuery = context.Request.Query["lang"];

            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                requestCulture = new CultureInfo(cultureQuery);
            }
            else
            {
                requestCulture = this.options.DefaultCulture;
            }

            if (requestCulture != null)
            {
                var culture = new CultureInfo(cultureQuery);
                /*#if !DNXCORE50
                                Thread.CurrentThread.CurrentCulture = culture;
                                Thread.CurrentThread.CurrentUICulture = culture;
                #else*/
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                // #endif
            }

            return this.next(context);
        }
    }
}
