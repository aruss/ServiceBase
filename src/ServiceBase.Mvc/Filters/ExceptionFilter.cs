// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc.Filters
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;

    public class ExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly bool _includeStackTrace;
        private readonly ILogger _logger;

        public ExceptionFilter(
            ILogger<ExceptionFilter> logger,
            IHostEnvironment hostEnvironment)
        {
            this._logger = logger;

            this._includeStackTrace =
                hostEnvironment.IsDevelopment() ||
                hostEnvironment.IsEnvironment("Test");
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