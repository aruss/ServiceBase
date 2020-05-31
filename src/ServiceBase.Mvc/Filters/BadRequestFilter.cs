// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class BadRequestFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is BadRequestObjectResult objResult)
            {
                objResult.Value = new ErrorModel
                {
                    Type = objResult.Value.GetType().Name,
                    Error = objResult.Value
                };
            }
            else if (context.Result is BadRequestResult result)
            {
                context.Result = new BadRequestObjectResult("");
            }
        }
    }
}