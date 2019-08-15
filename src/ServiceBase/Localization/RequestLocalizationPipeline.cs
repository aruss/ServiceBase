namespace ServiceBase.Localization
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Options;

    public class RequestLocalizationPipeline
    {
        public void Configure(
            IApplicationBuilder app,
            IOptions<RequestLocalizationOptions> options)
        {
            app.UseRequestLocalization(options.Value);
        }
    }
}
