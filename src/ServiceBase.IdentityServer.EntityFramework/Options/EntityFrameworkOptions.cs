using IdentityServer4.EntityFramework.Options;

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

    public class DefaultStoreOptions
    {
        public string DefaultSchema { get; set; } = null;

        public TableConfiguration UserAccount { get; set; } = new TableConfiguration("UserAccounts");
        public TableConfiguration ExternalAccount { get; set; } = new TableConfiguration("ExternalAccounts");
        public TableConfiguration UserClaim { get; set; } = new TableConfiguration("UserClaims");
    }
}
