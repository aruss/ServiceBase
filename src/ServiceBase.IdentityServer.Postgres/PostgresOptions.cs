namespace ServiceBase.IdentityServer.Postgres
{
    public class PostgresOptions
    {
        public string ConnectionString { get; set; }

        public bool CreateTablesAlways { get; set; } = false;
        public bool CreateTablesIfNecessary { get; set; } = true;
        public bool SeedExampleData { get; set; } = true;
    }
}
