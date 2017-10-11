namespace ServiceBase.Mvc
{
    using Microsoft.AspNetCore.Mvc;

    public static class ControllerExtensions
    {
        /// <summary>
        /// Returns a created HTTP status code according
        /// to https://tools.ietf.org/html/rfc7231#section-4.3.4
        /// </summary>
        /// <param name="controller">Instance of the
        /// <see cref="ControllerBase"/>.</param>
        /// <returns>Instance of <see cref="IActionResult"/></returns>
        public static IActionResult Created(this ControllerBase controller)
        {
            return new StatusCodeResult(201);
        }

        /// <summary>
        /// Adds an error to model state and returns the whole model state as
        /// BadRequest result.
        /// </summary>
        /// <param name="controller">Instance of the
        /// <see cref="ControllerBase"/>.</param>
        /// <param name="fieldName">Field name causing the problem.</param>
        /// <param name="errorMessage">Error message describing the problem.
        /// </param>
        /// <returns>Instance of <see cref="IActionResult"/></returns>
        public static IActionResult BadRequest(
            this ControllerBase controller,
            string fieldName,
            string errorMessage)
        {
            if (controller.ModelState
                .TryAddModelError(fieldName, errorMessage))
            {
                return controller.BadRequest(controller.ModelState);
            }

            return controller.BadRequest(errorMessage);
        }
    }
}
