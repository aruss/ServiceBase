using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using ServiceBase.IdentityServer.Services;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Postgres
{
    public class StoreInitializer : IStoreInitializer
    {
        private PostgresOptions _options;
        private ILogger<StoreInitializer> _logger;
        private IHostingEnvironment _env;

        public StoreInitializer(PostgresOptions options, ILogger<StoreInitializer> logger, IHostingEnvironment env)
        {
            _options = options;
            _logger = logger;
            _env = env;
        }

        public string GetSqlString(string resourceName)
        {
            var assembly = typeof(StoreInitializer).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(resourceName);
            var tr = new StreamReader(stream);
            var sql = tr.ReadToEnd();

            return sql;
        }

        public async Task Initialize()
        {
            _logger.LogInformation("Try open database connection");
            using (var con = new NpgsqlConnection(_options.ConnectionString))
            {
                con.Open();

                var result = await con.QuerySingleAsync<ExistsResult>("SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'useraccounts');");

                if (_options.CreateTablesIfNecessary)
                {
                    _logger.LogInformation("Create tables if necessary");
                    if (_options.CreateTablesAlways || !result.Exists)
                    {
                        _logger.LogInformation("Initializing database");

                        con.Execute(GetSqlString("ServiceBase.IdentityServer.Postgres.CreateTables.sql"));

                        if (_options.SeedExampleData)
                        {
                            _logger.LogInformation("Seeding test data");
                            con.Execute(GetSqlString("ServiceBase.IdentityServer.Postgres.ExampleData.sql"));
                        }
                    }

                    _logger.LogInformation("Database looks great");
                }
                else
                {
                    if (!result.Exists)
                    {
                        _logger.LogError("Database does not contain required tables");
                    }
                }

                con.Close();
            };
        }

        public class ExistsResult
        {
            public bool Exists { get; set; }
        }
    }
}


