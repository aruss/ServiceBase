namespace ServiceBase.Mvc.Theming
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Provides a <see cref="ThemeInfoResult"/>.
    /// </summary>
    public interface IThemeInfoProvider
    {
        // TODO: Remove the HttpContext dependency 

        /// <summary>
        /// Get an instance of <see cref="ThemeInfoResult"/>.
        /// </summary>
        /// <returns>An instance of <see cref="ThemeInfoResult"/>.</returns>
        Task<ThemeInfoResult> GetThemeInfoResultAsync();
    }
}
