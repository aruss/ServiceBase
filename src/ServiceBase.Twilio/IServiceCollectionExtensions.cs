using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBase.Notification.SMS;

namespace ServiceBase.Notification.Twilio
{
    public static class IServiceCollectionExtensions
    {
        public static void AddTwillioSmsSender(this IServiceCollection services, IConfigurationSection config)
        {
            services.Configure<TwillioOptions>(config);
            services.AddTransient<ISmsSender, TwillioSmsSender>();
        }        
    }
}
