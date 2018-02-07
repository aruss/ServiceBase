namespace ServiceBase.Mvc.Theming
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public interface IRequestThemeInfoProvider
    {
        Task<ThemeInfoResult> DetermineThemeInfoResult(HttpContext httpContext);
    }
}
