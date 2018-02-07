namespace ServiceBase.Mvc.Theming
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore.Mvc.Razor;
    using ServiceBase.Extensions;

    /// <summary>
    /// Specifies the contracts for a view location expander that is used by 
    /// <see cref="RazorViewEngine"/> instances to determine search paths for a view.
    /// NOTE: FileWatcher will not track changes if absolute path is provided
    /// due to #248 bug <see href="https://github.com/aspnet/FileSystem/issues/248"/>
    /// </summary>
    public class ThemeViewLocationExpander : IViewLocationExpander
    {
        private readonly IRequestThemeInfoProvider _themeInfoProvider;
        private readonly string _basePath;

        public ThemeViewLocationExpander(
            string basePath,
            IRequestThemeInfoProvider themeInfoProvider)
        {
            this._themeInfoProvider = themeInfoProvider;

            if (Path.IsPathRooted(basePath))
            {
                this._basePath = Path
                    .GetFullPath(basePath)
                    .RemoveTrailingSlash();
            }
            else
            {
                this._basePath = basePath
                    .Replace("./", "~/")
                    .RemoveTrailingSlash();
            }
        }

        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            string requestTheme = context.Values["requestTheme"];
            string defaultTheme = context.Values["defaultTheme"];

            yield return $"{this._basePath}/{requestTheme}/Views/{{1}}/{{0}}.cshtml";
            yield return $"{this._basePath}/{requestTheme}/Views/Shared/{{0}}.cshtml";

            if (requestTheme != defaultTheme)
            {
                yield return $"{this._basePath}/{defaultTheme}/Views/{{1}}/{{0}}.cshtml";
                yield return $"{this._basePath}/{defaultTheme}/Views/Shared/{{0}}.cshtml";
            }
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            ThemeInfoResult result = this._themeInfoProvider
                .DetermineThemeInfoResult(context.ActionContext.HttpContext)
                .Result;

            context.Values["requestTheme"] = result.RequestTheme;
            context.Values["defaultTheme"] = result.DefaultTheme;
        }
    }
}
