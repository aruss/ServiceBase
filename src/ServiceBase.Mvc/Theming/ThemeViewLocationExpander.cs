namespace ServiceBase.Mvc.Theming
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Razor;

    /// <summary>
    /// Specifies the contracts for a view location expander that is used by 
    /// <see cref="RazorViewEngine"/> instances to determine search paths for a view.
    /// NOTE: FileWatcher will not track changes if absolute path is provided
    /// due to #2546 bug <see href="https://github.com/aspnet/Home/issues/2546"/>
    /// </summary>
    public class ThemeViewLocationExpander : IViewLocationExpander
    {
        private readonly IThemeInfoProvider _themeInfoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeViewLocationExpander"/> class.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="themeInfoProvider"></param>
        public ThemeViewLocationExpander(
            IThemeInfoProvider themeInfoProvider)
        {
            this._themeInfoProvider = themeInfoProvider;
        }

        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            string requestTheme = context.Values["requestTheme"];
            string defaultTheme = context.Values["defaultTheme"];

            yield return $"~/Plugins/{requestTheme}/Views/{{1}}/{{0}}.cshtml";
            yield return $"~/Plugins/{requestTheme}/Views/Shared/{{0}}.cshtml";

            if (requestTheme != defaultTheme)
            {
                yield return $"~/Plugins/{defaultTheme}/Views/{{1}}/{{0}}.cshtml";
                yield return $"~/Plugins/{defaultTheme}/Views/Shared/{{1}}/{{0}}.cshtml";
            }
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {   
            ThemeInfoResult result = this._themeInfoProvider
                .GetThemeInfoResultAsync()
                .Result;

            context.Values["requestTheme"] = result.RequestTheme;
            context.Values["defaultTheme"] = result.DefaultTheme;
        }
    }
}
