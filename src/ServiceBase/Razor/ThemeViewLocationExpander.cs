namespace ServiceBase.Razor
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Razor;
    using ServiceBase.Extensions;

    /// <summary>
    /// Specifies the contracts for a view location expander that is used by 
    /// <see cref="Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine"/> instances 
    /// to determine search paths for a view.
    /// </summary>
    public class ThemeViewLocationExpander : IViewLocationExpander
    {
        private readonly string _themePath;

        public ThemeViewLocationExpander(string themePath)
        {
            this._themePath = themePath?.RemoveTrailingSlash();
        }

        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            yield return $"{this._themePath}/Views/{{1}}/{{0}}.cshtml";
            yield return $"{this._themePath}/Views/Shared/{{0}}.cshtml";
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {

        }
    }
}
