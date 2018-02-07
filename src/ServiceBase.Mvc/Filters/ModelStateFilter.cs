namespace ServiceBase.Mvc.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Returns Bad Request response in case the model state is invalid.
    /// </summary>
    public class ModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result =
                    new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}