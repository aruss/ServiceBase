// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class StoreModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(
            ActionExecutedContext filterContext)
        {
            // Only export when ModelState is not valid
            if (!filterContext.ModelState.IsValid)
            {
                // Export if we are redirecting
                if (filterContext.Result is RedirectResult
                    || filterContext.Result is RedirectToRouteResult
                    || filterContext.Result is RedirectToActionResult)
                {
                    Controller controller =
                        filterContext.Controller as Controller;

                    if (controller != null &&
                        filterContext.ModelState != null)
                    {
                        string modelState = ModelStateHelper
                            .SerialiseModelState(filterContext.ModelState);

                        controller.TempData[ModelStateHelper.Key] =
                            modelState;
                    }
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
