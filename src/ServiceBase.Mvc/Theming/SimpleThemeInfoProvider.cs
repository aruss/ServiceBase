namespace ServiceBase.Mvc.Theming
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class SimpleThemeInfoProvider : IRequestThemeInfoProvider
    {
        private readonly ThemeInfoResult _themeInfoResult;

        public SimpleThemeInfoProvider(string themeName)
        {
            this._themeInfoResult = new ThemeInfoResult
            {
                DefaultTheme = themeName,
                RequestTheme = themeName
            }; 
        }

        public Task<ThemeInfoResult> DetermineThemeInfoResult(
            HttpContext httpContext)
        {
            return Task.FromResult(this._themeInfoResult); 
        }
    }
}
