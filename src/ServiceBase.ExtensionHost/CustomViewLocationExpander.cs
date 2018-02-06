namespace ServiceBase.ExtensionHost
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Razor;

    public class CustomViewLocationExpander : IViewLocationExpander
    {
        private readonly IRequestThemeInfoProvider _themeInfoProvider;

        public CustomViewLocationExpander(IRequestThemeInfoProvider themeInfoProvider)
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

            yield return $"~/Plugins/{defaultTheme}Views/{{1}}/{{0}}.cshtml";
            yield return $"~/Plugins/{defaultTheme}/ Views/Shared/{{0}}.cshtml";
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // TODO: Read from config
            var result = this._themeInfoProvider
                .DetermineThemeInfoResult(context.ActionContext.HttpContext)
                .Result;

            context.Values["requestTheme"] = result.RequestTheme;
            context.Values["defaultTheme"] = result.DefaultTheme;
        }
    }

    public class ThemeInfoResult
    {
        public string RequestTheme { get; set; }
        public string DefaultTheme { get; set; }
    }

    public interface IRequestThemeInfoProvider
    {
        Task<ThemeInfoResult> DetermineThemeInfoResult(HttpContext httpContext);
    }
    
    public class DefaultRequestThemeInfoProvider : IRequestThemeInfoProvider
    {
        public async Task<ThemeInfoResult> DetermineThemeInfoResult(HttpContext httpContext)
        {
            var result = new ThemeInfoResult
            {
                RequestTheme = httpContext.Request.Query["theme"],
                DefaultTheme = "BaseTheme" // TODO: read from config
            };

            // TODO: check if plugin exists 

            if (string.IsNullOrWhiteSpace(result.RequestTheme))
            {
                result.RequestTheme = result.DefaultTheme;
            }

            return result;
        }
    }
}
