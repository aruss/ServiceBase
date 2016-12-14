﻿using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.EntityFramework.Configuration;
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
        private readonly DefaultDbContext _defaultDbContext;
        private readonly ICrypto _crypto;
        private readonly ApplicationOptions _applicationOptions;

        public DefaultStoreInitializer(
            IOptions<EntityFrameworkOptions> options,
            ILogger<DefaultStoreInitializer> logger,
            IHostingEnvironment env,
            ConfigurationDbContext configurationDbContext,
            PersistedGrantDbContext persistedGrantDbContext,
            DefaultDbContext defaultDbContext,
            ICrypto crypto,
            IOptions<ApplicationOptions> applicationOptions)
        {
            _options = options.Value;
            _logger = logger;
            _env = env;
            _configurationDbContext = configurationDbContext;
            _persistedGrantDbContext = persistedGrantDbContext;
            _defaultDbContext = defaultDbContext;
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
                this.SeedClients();
                this.SeedScopes();
                this.SeedUserAccounts();
            }
        }

        internal virtual void MigrateDatabase()
        {
            _configurationDbContext.Database.Migrate();
            _persistedGrantDbContext.Database.Migrate();
            _defaultDbContext.Database.Migrate();
        }

        internal virtual void SeedClients()
        {
            if (!_configurationDbContext.Clients.Any())
            {
                foreach (var client in Clients.Get())
                {
                    _configurationDbContext.Clients.Add(client.ToEntity());
                }
                _configurationDbContext.SaveChanges();
            }
        }

        internal virtual void SeedScopes()
        {
            if (!_configurationDbContext.Scopes.Any())
            {
                foreach (var client in Scopes.Get())
                {
                    _configurationDbContext.Scopes.Add(client.ToEntity());
                }
                _configurationDbContext.SaveChanges();
            }
        }

        internal virtual void SeedUserAccounts()
        {
            if (!_defaultDbContext.UserAccounts.Any())
            {
                foreach (var userAccount in UserAccounts.Get(_crypto, _applicationOptions))
                {
                    _defaultDbContext.UserAccounts.Add(userAccount.ToEntity());
                }
                _defaultDbContext.SaveChanges();
            }
        }
    }
}


