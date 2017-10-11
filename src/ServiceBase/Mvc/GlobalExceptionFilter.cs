﻿namespace ServiceBase.Mvc
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    public class GlobalExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly bool _includeStackTrace;
        private readonly ILogger _logger;

        public GlobalExceptionFilter(
            ILogger logger,
            bool includeStackTrace = false)
        {
            this._logger = logger;
            this._includeStackTrace = includeStackTrace; 
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

            if (this._includeStackTrace)
            {
                response.StackTrace = context.Exception.StackTrace;
            }

            this._logger.LogError(context.Exception,
                context.Exception.Message);

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500,
                DeclaredType = typeof(ErrorModel)
            };
        }
    }
}
