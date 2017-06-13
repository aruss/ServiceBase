using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceBase.Api
{
    public class ApiResult<TResult> : ApiResult
    {
        public TResult Result { get; set; }
    }

    public class ApiResult
    {
        public bool Success { get; set; } = true;
        public List<ResponseMessage> Messages { get; set; }

    }

    public class ExceptionApiResult : ApiResult
    {
        public ExceptionApiResult(Exception ex)
        {
            this.Success = false;
            this.Exception = new ExceptionDescription
            {
                Type = ex.GetType().Name,
                Description = ex.Message
            };
        }

        public ExceptionDescription Exception { get; set; }
    }

    public class InvalidStateApiResult : ApiResult
    {
        public InvalidStateApiResult(ModelStateDictionary modelState)
        {
            foreach (var ms in modelState)
            {
                foreach (var err in ms.Value.Errors)
                {
                    this.AddMessage(err.ErrorMessage, ResponseMessageKind.Error, ms.Key.Camelize());
                }
            }

            this.Success = false;
        }

        public InvalidStateApiResult(string message, ResponseMessageKind kind = ResponseMessageKind.Info, string fieldName = null)
        {
            this.Success = false;
            this.AddMessage(message, kind, fieldName.Camelize());
        }

        public InvalidStateApiResult()
        {
            this.Success = false;
        }
    }
}