using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBase.IdentityServer.Services;
using System;

namespace ServiceBase.IdentityServer.Postgres
{
    public static class IServiceCollectionExtensions
    {
        public static void AddPostgresStores(this IServiceCollection services, Action<PostgresOptions> configure)
        {
            services.Configure<PostgresOptions>(configure);
            ConfigureServices(services); 
        }

        public static void AddPostgresStores(this IServiceCollection services, IConfigurationSection section)
        {
            services.Configure<PostgresOptions>(section);
            ConfigureServices(services);
        }

        internal static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IUserAccountStore, UserAccountStore>();
            services.AddTransient<IStoreInitializer, StoreInitializer>();

            /*services.AddScoped<Npgsql.NpgsqlConnection>((s) =>
            {
                var connection = new Npgsql.NpgsqlConnection(options.ConnectionString);
                connection.Open();

                return connection;
            });*/
        }
    }
}
