using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBase.IdentityServer.Services;
using System;

namespace ServiceBase.IdentityServer.EntityFramework
{
    public static class IServiceCollectionExtensions
    {
        public static void AddEntityFrameworkStores(this IServiceCollection services, Action<EntityFrameworkOptions> configure)
        {
            services.Configure<EntityFrameworkOptions>(configure);
            ConfigureServices(services);
        }

        public static void AddEntityFrameworkStores(this IServiceCollection services, IConfigurationSection section)
        {
            services.Configure<EntityFrameworkOptions>(section);
            ConfigureServices(services);
        }

        internal static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IUserAccountStore, UserAccountStore>();
            services.AddTransient<IStoreInitializer, StoreInitializer>();

        }
    }
}
