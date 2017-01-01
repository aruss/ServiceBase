using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBase.IdentityServer.Configuration;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.EntityFramework.DbContexts;
using ServiceBase.IdentityServer.EntityFramework.Mappers;
using ServiceBase.IdentityServer.EntityFramework.Options;
using ServiceBase.IdentityServer.Services;
using System.Linq;

namespace ServiceBase.IdentityServer.EntityFramework
{
    public class DefaultStoreInitializer : IStoreInitializer
    {
        private readonly EntityFrameworkOptions _options;
        private readonly ILogger<DefaultStoreInitializer> _logger;
        private readonly IHostingEnvironment _env;
        private readonly ConfigurationDbContext _configurationDbContext;
        private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly UserAccountDbContext _userAccountDbContext;
        private readonly ICrypto _crypto;
        private readonly ApplicationOptions _applicationOptions;

        public DefaultStoreInitializer(
            IOptions<EntityFrameworkOptions> options,
            ILogger<DefaultStoreInitializer> logger,
            IHostingEnvironment env,
            ConfigurationDbContext configurationDbContext,
            PersistedGrantDbContext persistedGrantDbContext,
            UserAccountDbContext userAccountDbContext,
            ICrypto crypto,
            IOptions<ApplicationOptions> applicationOptions)
        {
            _options = options.Value;
            _logger = logger;
            _env = env;
            _configurationDbContext = configurationDbContext;
            _persistedGrantDbContext = persistedGrantDbContext;
            _userAccountDbContext = userAccountDbContext;
            _crypto = crypto;
            _applicationOptions = applicationOptions.Value;
        }

        public void InitializeStores()
        {
            if (_options.MigrateDatabase)
            {
                this.MigrateDatabase();
            }

            if (_options.SeedExampleData)
            {
                this.EnsureSeedData();
            }
        }

        internal virtual void MigrateDatabase()
        {
            _configurationDbContext.Database.Migrate();
            _persistedGrantDbContext.Database.Migrate();
            _userAccountDbContext.Database.Migrate();
        }

        internal virtual void EnsureSeedData()
        {
            if (!_configurationDbContext.Clients.Any())
            {
                foreach (var client in Clients.Get().ToList())
                {
                    _configurationDbContext.Clients.Add(client.ToEntity());
                }
                _configurationDbContext.SaveChanges();
            }

            if (!_configurationDbContext.IdentityResources.Any())
            {
                foreach (var resource in Resources.GetIdentityResources().ToList())
                {
                    _configurationDbContext.IdentityResources.Add(resource.ToEntity());
                }
                _configurationDbContext.SaveChanges();
            }

            if (!_configurationDbContext.ApiResources.Any())
            {
                foreach (var resource in Resources.GetApiResources().ToList())
                {
                    _configurationDbContext.ApiResources.Add(resource.ToEntity());
                }
                _configurationDbContext.SaveChanges();
            }

            if (!_userAccountDbContext.UserAccounts.Any())
            {
                foreach (var userAccount in UserAccounts.Get(_crypto, _applicationOptions).ToList())
                {
                    _userAccountDbContext.UserAccounts.Add(userAccount.ToEntity());
                }
                _userAccountDbContext.SaveChanges();
            }
        }
    }
}