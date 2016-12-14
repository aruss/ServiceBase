using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Options;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.EntityFramework.Configuration;
using ServiceBase.IdentityServer.EntityFramework.DbContexts;
using ServiceBase.IdentityServer.EntityFramework.Mappers;
using ServiceBase.IdentityServer.Models;
using ServiceBase.IdentityServer.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ServiceBase.IdentityServer.UnitTests
{
    public class TestStoreInitializer : IStoreInitializer
    {
        private readonly ConfigurationDbContext _configurationDbContext;
        private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly DefaultDbContext _defaultDbContext;
        private readonly ICrypto _crypto;
        private readonly ApplicationOptions _applicationOptions;

        public TestStoreInitializer(
            ConfigurationDbContext configurationDbContext,
            PersistedGrantDbContext persistedGrantDbContext,
            DefaultDbContext defaultDbContext,
            ICrypto crypto,
             IOptions<ApplicationOptions> applicationOptions)
        {
            _configurationDbContext = configurationDbContext;
            _persistedGrantDbContext = persistedGrantDbContext;
            _defaultDbContext = defaultDbContext;
            _crypto = crypto;
            _applicationOptions = applicationOptions.Value;
        }

        public void InitializeStores()
        {

            foreach (var client in Clients.Get())
            {
                _configurationDbContext.Clients.Add(client.ToEntity());
            }
            _configurationDbContext.SaveChanges();

            foreach (var client in Scopes.Get())
            {
                _configurationDbContext.Scopes.Add(client.ToEntity());
            }
            _configurationDbContext.SaveChanges();


            var now = DateTime.UtcNow;
            var userAccounts = new List<UserAccount>
            {
                new UserAccount
                {
                    Id = Guid.Parse("0c2954d2-4c73-44e3-b0f2-c00403e4adef"),
                    Email = "alice@localhost",
                    PasswordHash  = _crypto.HashPassword("alice@localhost", _applicationOptions.PasswordHashingIterationCount),
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsEmailVerified = true,
                    IsLoginAllowed = true,
                    Claims = new List<UserClaim>
                    {
                        new UserClaim(JwtClaimTypes.Name, "Alice Smith"),
                        new UserClaim(JwtClaimTypes.GivenName, "Alice"),
                        new UserClaim(JwtClaimTypes.FamilyName, "Smith"),
                        new UserClaim(JwtClaimTypes.Email, "alice@localhost"),
                        new UserClaim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new UserClaim(JwtClaimTypes.Role, "Admin"),
                        new UserClaim(JwtClaimTypes.Role, "Geek"),
                        new UserClaim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new UserClaim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                    }
                },
                new UserAccount
                {
                    Id = Guid.Parse("28575826-68a0-4a1d-9428-674a2eb5db95"),
                    Email = "bob@localhost",
                    PasswordHash  = _crypto.HashPassword("bob@localhost", _applicationOptions.PasswordHashingIterationCount),
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsEmailVerified = true,
                    IsLoginAllowed = true,
                    Claims = new List<UserClaim>
                    {
                        new UserClaim(JwtClaimTypes.Name, "Bob Smith"),
                        new UserClaim(JwtClaimTypes.GivenName, "Bob"),
                        new UserClaim(JwtClaimTypes.FamilyName, "Smith"),
                        new UserClaim(JwtClaimTypes.Email, "bob@localhost"),
                        new UserClaim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new UserClaim(JwtClaimTypes.Role, "Developer"),
                        new UserClaim(JwtClaimTypes.Role, "Geek"),
                        new UserClaim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new UserClaim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                    }
                }
            };

            foreach (var userAccount in userAccounts)
            {
                foreach (var claim in userAccount.Claims)
                {
                    claim.UserAccountId = userAccount.Id;
                }

                _defaultDbContext.UserAccounts.Add(userAccount.ToEntity());
            }

            _defaultDbContext.SaveChanges();
        }
    }
}
