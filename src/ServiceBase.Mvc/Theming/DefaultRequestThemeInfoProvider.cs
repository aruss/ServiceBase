namespace ServiceBase.Mvc.Theming
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class DefaultRequestThemeInfoProvider : IThemeInfoProvider
    {
        private readonly string _defaultTheme;

        public DefaultRequestThemeInfoProvider(
            string defaultTheme = "DefaultTheme")
        {
            this._defaultTheme = defaultTheme; 
        }

        public Task<ThemeInfoResult> DetermineThemeInfoResult(
            HttpContext httpContext)
        {
            var result = new ThemeInfoResult
            {
                RequestTheme = httpContext.Request.Query["theme"],
                DefaultTheme = this._defaultTheme
            };

            // TODO: check if plugin exists
            // TODO: check if plugin is a theme

            if (string.IsNullOrWhiteSpace(result.RequestTheme))
            {
                result.RequestTheme = result.DefaultTheme;
            }

            return Task.FromResult(result); 
        }
    }
}
