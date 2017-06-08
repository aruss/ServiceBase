using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ServiceBase.Api
{
    public class ApiResultExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ApiResultExceptionFilterAttribute(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            if (!_hostingEnvironment.IsDevelopment())
            {
                // Do nothing
                return;
            }

            context.Result = new JsonResult(new ExceptionApiResult(context.Exception))
            {
                StatusCode = 500
            };
        }
    }
}
