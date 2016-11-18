using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ServiceBase.IdentityServer.EntityFramework.Entities;
using System.Threading.Tasks;
using System;
using ServiceBase.IdentityServer.EntityFramework.Options;
using ServiceBase.IdentityServer.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;

namespace ServiceBase.IdentityServer.EntityFramework.DbContexts
{
    /*
     * 
    public class TemporaryDbContextFactory : IDbContextFactory<DefaultDbContext>
    {

        public DefaultDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<DefaultDbContext>();
            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=pinchdb;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new DefaultDbContext(builder.Options, new EntityFrameworkOptions());
        }
    }

    public class TemporaryDbContext2Factory : IDbContextFactory<ConfigurationDbContext>
    {

        public ConfigurationDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();
            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=pinchdb;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new ConfigurationDbContext(builder.Options, new ConfigurationStoreOptions());
        }
    }
    */

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
