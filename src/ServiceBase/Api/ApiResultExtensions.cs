using System.Collections.Generic;

namespace ServiceBase.Api
{
    public static class ApiResultExtensions
    {
        public static void AddMessage(
            this ApiResult apiResult,
            string message, 
            ResponseMessageKind kind = ResponseMessageKind.Info, 
            string fieldName = null)
        {
            if (apiResult.Messages == null)
            {
                apiResult.Messages = new List<ResponseMessage>();
            }

            apiResult.Messages.Add(new ResponseMessage
            {
                Kind = kind,
                Field = fieldName.Camelize(),
                Message = message
            });
        }
    }
}   
