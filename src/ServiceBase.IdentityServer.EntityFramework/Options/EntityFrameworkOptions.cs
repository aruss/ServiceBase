namespace ServiceBase.IdentityServer.EntityFramework.Options
{
    public class EntityFrameworkOptions
    {
        public string ConnectionString { get; set; }

        public bool CreateTablesAlways { get; set; } = false;
        public bool CreateTablesIfNecessary { get; set; } = true;
        public bool SeedExampleData { get; set; } = true;

        public string TablePrefix { get; set; }
    }
}
