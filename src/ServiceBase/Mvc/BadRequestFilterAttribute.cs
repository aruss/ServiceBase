namespace ServiceBase.Mvc
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class BadRequestFilterAttribute : ActionFilterAttribute
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
        }
    }
}
