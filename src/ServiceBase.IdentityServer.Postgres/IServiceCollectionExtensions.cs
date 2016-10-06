using Microsoft.Extensions.DependencyInjection;
using ServiceBase.IdentityServer.Services;

namespace ServiceBase.IdentityServer.Postgres
{
    public static class IServiceCollectionExtensions
    {
        public static void AddPostgres(this IServiceCollection services, string connectionStirng)
        {
            services.AddSingleton(new PostgresOptions
            {
                ConnectionString = connectionStirng
            });

            /*services.AddScoped<Npgsql.NpgsqlConnection>((s) =>
            {
                var connection = new Npgsql.NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                connection.Open();

                return connection;
            });*/
            
            services.AddTransient<IUserAccountStore, UserAccountStore>();
        }
    }
}
