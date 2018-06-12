namespace ServiceBase.Mvc.Theming
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Razor;
    using ServiceBase.Extensions;
    using System.IO;
    using System;

    /// <summary>
    /// Specifies the contracts for a view location expander that is used by 
    /// <see cref="RazorViewEngine"/> instances to determine search paths for a view.
    /// NOTE: FileWatcher will not track changes if absolute path is provided
    /// due to #2546 bug <see href="https://github.com/aspnet/Home/issues/2546"/>
    /// </summary>
    public class ThemeViewLocationExpander : IViewLocationExpander
    {
        private readonly IThemeInfoProvider _themeInfoProvider;
        private readonly string _template1;
        private readonly string _template2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeViewLocationExpander"/> class.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="themeInfoProvider"></param>
        public ThemeViewLocationExpander(
            IThemeInfoProvider themeInfoProvider,
            string basePath)
        {
            this._themeInfoProvider = themeInfoProvider;

            if (Path.IsPathRooted(basePath))
            {
                throw new NotImplementedException(
                  "Support for absolute pathes is not yet implemeted");

                // this._template1 = Path.GetFullPath(Path.Combine(
                //     basePath, "{0}", "Views", "{{1}}", "{{0}}.cshtml"));
                // 
                // this._template2 = Path.GetFullPath(Path.Combine(
                //     basePath, "{0}", "Views", "Shared", "{{0}}.cshtml"));
            }
            else
            {
                basePath = basePath
                    .Replace("./", "~/")
                    .RemoveTrailingSlash();

                this._template1 =
                    $"{basePath}/{{0}}/Views/{{{{1}}}}/{{{{0}}}}.cshtml";

                this._template2 =
                    $"{basePath}/{{0}}/Views/Shared/{{{{0}}}}.cshtml";
            }
        }

        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            string requestTheme = context.Values["RequestTheme"];
            string defaultTheme = context.Values["DefaultTheme"];

            yield return String.Format(this._template1, requestTheme);
            yield return String.Format(this._template2, requestTheme);

            if (requestTheme != defaultTheme)
            {
                yield return String.Format(this._template1, defaultTheme);
                yield return String.Format(this._template2, defaultTheme);
            }
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            ThemeInfoResult result = this._themeInfoProvider
                .GetThemeInfoResultAsync()
                .Result;

            context.Values["RequestTheme"] = result.RequestTheme;
            context.Values["DefaultTheme"] = result.DefaultTheme;
        }
    }
}
