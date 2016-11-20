using Microsoft.EntityFrameworkCore;
using ServiceBase.IdentityServer.EntityFramework.Entities;
using ServiceBase.IdentityServer.EntityFramework.Extensions;
using ServiceBase.IdentityServer.EntityFramework.Options;
using System;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.EntityFramework.DbContexts
{
    public class DefaultDbContext : DbContext
    {
        private readonly DefaultStoreOptions _options;
        
        public DefaultDbContext(DbContextOptions<DefaultDbContext> dbContextOptions, DefaultStoreOptions options)
            : base(dbContextOptions)
        {
            if (options == null) throw new ArgumentNullException(nameof(_options));
            _options = options;
        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<ExternalAccount> ExternalAccounts { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureDefaultStore(_options);

            base.OnModelCreating(modelBuilder);
        }
    }
}
