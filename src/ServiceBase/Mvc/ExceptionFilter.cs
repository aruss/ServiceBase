namespace ServiceBase.Mvc
{
    using System;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    public class ExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly bool includeStackTrace;
        private readonly ILogger logger;

        public ExceptionFilter(
            ILogger<ExceptionFilter> logger,
            IHostingEnvironment hostingEnvironment)
        {
            this.logger = logger;

            this.includeStackTrace =
                hostingEnvironment.IsDevelopment() ||
                hostingEnvironment.IsEnvironment("Test");
        }

        public void Dispose()
        {
        }

        public void OnException(ExceptionContext context)
        {
            ErrorModel response = new ErrorModel
            {
                Type = context.Exception.GetType().Name,
                Error = context.Exception.Message
            };

            if (this.includeStackTrace)
            {
                response.StackTrace = context.Exception.StackTrace;
            }

            this.logger.LogError(context.Exception,
                context.Exception.Message);

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500,
                DeclaredType = typeof(ErrorModel)
            };
        }
    }
}