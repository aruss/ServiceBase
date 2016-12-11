using Microsoft.EntityFrameworkCore;
using System;

namespace ServiceBase.IdentityServer.EntityFramework.Options
{
    public class EntityFrameworkOptions
    {
        public string ConnectionString { get; set; }

        public bool SeedExampleData { get; set; } = false;
        public bool MigrateDatabase { get; set; } = false;

        public string TablePrefix { get; set; }
    }
}
