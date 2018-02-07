namespace ServiceBase.Mvc.Theming
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore.Mvc.Razor;
    using ServiceBase.Extensions;

    /// <summary>
    /// Specifies the contracts for a view location expander that is used by 
    /// <see cref="Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine"/> instances 
    /// to determine search paths for a view.
    /// NOTE: FileWatcher will not track changes if absolute path is provided
    /// due to #248 bug <see href="https://github.com/aspnet/FileSystem/issues/248"/>
    /// </summary>
    public class SimpleThemeViewLocationExpander : IViewLocationExpander
    {
        private readonly string _themePath;

        public SimpleThemeViewLocationExpander(string themePath)
        {
            if (Path.IsPathRooted(themePath))
            {
                this._themePath = Path
                    .GetFullPath(themePath)
                    .RemoveTrailingSlash();
            }
            else
            {
                this._themePath = themePath
                    .Replace("./", "~/")
                    .RemoveTrailingSlash();
            }
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
