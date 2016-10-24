using Microsoft.Extensions.DependencyInjection;
using ServiceBase.IdentityServer.Services;
using System;

namespace ServiceBase.IdentityServer.Postgres
{
    public static class IServiceCollectionExtensions
    {
        public static void AddPostgresStores(this IServiceCollection services, Action<PostgresOptions> configure)
        {
            var options = new PostgresOptions();

            configure(options);

            services.AddSingleton(options);
            services.AddTransient<IUserAccountStore, UserAccountStore>();

            /*services.AddScoped<Npgsql.NpgsqlConnection>((s) =>
            {
                var connection = new Npgsql.NpgsqlConnection(options.ConnectionString);
                connection.Open();

                return connection;
            });*/
        }
    }
}
