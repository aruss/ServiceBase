namespace ServiceBase.Razor
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Razor;

    /// <summary>
    /// Specifies the contracts for a view location expander that is used by
    /// <see cref="Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine"/> instances
    /// to determine search paths for a view.
    /// </summary>
    public class CustomViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            yield return "~/Actions/{1}/Views/{0}.cshtml";
            yield return "~/Actions/SharedViews/{0}.cshtml";
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}