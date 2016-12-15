using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
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

namespace ServiceBase.IdentityServer.UnitTests
{
    /*
    cdb7b8c3-ccbc-486e-a265-b0b7426fb005
fafe443f-0829-4be1-9b0f-78dabfcf6c97
7f741d3e-8a57-4b7e-9f5c-f611d2d67e03
1e953397-b552-4970-8c8c-c45a1a2a663e
579d13a5-6c98-4c42-944a-823a2045805e
e5771c9a-4f19-45e7-be31-740aa825f9f7
efe37d26-4075-4d7a-8c92-9dba1bbe1d47
03219d06-9d07-4a6b-8180-807f4101bae1
2e0025a0-6ba9-498d-b6e6-44140dd6d923
cfd16155-63ce-4528-8bfc-d427d724f163
711dec2a-5390-4c4e-b8e9-9a40d704db5f
da39fc0b-5819-4d1e-aa22-aa6e26c7b46d
a56fb5cc-7190-4d3b-969d-25ae5c2f9742
1a41bb4c-f053-4c16-bd35-e12ad5dfca0a
9fb45c8d-b031-4e41-b82e-dd104fa90fad
2f3b216d-95f4-4b01-bd9f-a412c4b15d26

     */
    internal static class DbContextExtensions
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }

    public class TestStoreInitializer : IStoreInitializer, IDisposable
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

        public void Dispose()
        {

        }

        private List<UserClaim> CreateClaims(string name, string givenName, string familyName)
        {
            return new List<UserClaim>
            {
                new UserClaim(JwtClaimTypes.Name, name),
                new UserClaim(JwtClaimTypes.GivenName, givenName),
                new UserClaim(JwtClaimTypes.FamilyName, familyName)
            };
        }

        public void InitializeStores()
        {
            // Cleanup
            // HACK: the memory db survives server dispose so i have to cleanup it here
            //       cannot run this tests in parallel
            _configurationDbContext.Clients.Clear();
            _configurationDbContext.Scopes.Clear();
            _configurationDbContext.SaveChanges();
            _persistedGrantDbContext.PersistedGrants.Clear();
            _persistedGrantDbContext.SaveChanges();
            _defaultDbContext.UserAccounts.Clear();
            _defaultDbContext.ExternalAccounts.Clear();
            _defaultDbContext.UserClaims.Clear();
            _defaultDbContext.SaveChanges();

            // Add default sample data clients
            foreach (var client in Clients.Get())
            {
                _configurationDbContext.Clients.Add(client.ToEntity());
            }
            _configurationDbContext.SaveChanges();

            // Add default sample data scopes
            foreach (var client in Scopes.Get())
            {
                _configurationDbContext.Scopes.Add(client.ToEntity());
            }
            _configurationDbContext.SaveChanges();

            // Add test user accounts
            var now = DateTime.UtcNow;
            var userAccounts = new List<UserAccount>
            {
                // Active user account with local account but no external accounts
                new UserAccount
                {
                    Id = Guid.Parse("0c2954d2-4c73-44e3-b0f2-c00403e4adef"),
                    Email = "alice@localhost",
                    PasswordHash  = _crypto.HashPassword("alice@localhost", _applicationOptions.PasswordHashingIterationCount),
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsEmailVerified = true,
                    EmailVerifiedAt = now,
                    IsLoginAllowed = true,
                    Claims = this.CreateClaims("Alice Smith", "Alice", "Smith")
                },

                // Inactive user account with local account but no external accounts
                new UserAccount
                {
                    Id = Guid.Parse("6b13d17c-55a6-482e-96b9-dc784015f927"),
                    Email = "jim@localhost",
                    PasswordHash  = _crypto.HashPassword("jim@localhost", _applicationOptions.PasswordHashingIterationCount),
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsEmailVerified = true,
                    EmailVerifiedAt = now,
                    IsLoginAllowed = false,
                    Claims = new List<UserClaim>
                    {
                        new UserClaim(JwtClaimTypes.Name, "Jim Panse"),
                        new UserClaim(JwtClaimTypes.GivenName, "Jim"),
                        new UserClaim(JwtClaimTypes.FamilyName, "Panse")
                    }
                },

                // Not verified user account with local account but no external accounts
                new UserAccount
                {
                    Id = Guid.Parse("13808d08-b1c0-4f28-8d3e-8c9a4051efcb"),
                    Email = "paul@localhost",
                    PasswordHash  = _crypto.HashPassword("paul@localhost", _applicationOptions.PasswordHashingIterationCount),
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsEmailVerified = false,
                    IsLoginAllowed = false,
                    Claims = this.CreateClaims("Paul Panzer", "Paul", "Panzer")
                    // TODO: set VerificationKey, VerificationPurpose, VerificationKeySentAt
                },

                // External user account
                new UserAccount
                {
                    Id = Guid.Parse("58631b04-9be5-454a-aa1d-f679cd454fa6"),
                    Email = "bob@localhost",
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsEmailVerified = false,
                    IsLoginAllowed = false,
                    Claims = this.CreateClaims("Bob Smith", "Bob", "Smith"),
                    Accounts = new List<ExternalAccount>()
                    {
                        new ExternalAccount
                        {
                            CreatedAt = now,
                            Email = "bob@localhost",
                            Subject = "123456789",
                            Provider = "facebook"
                        }
                    }
                }
            };

            // Map all references
            foreach (var userAccount in userAccounts)
            {
                foreach (var claim in userAccount.Claims)
                {
                    claim.UserAccountId = userAccount.Id;
                    claim.UserAccount = userAccount;
                }

                if (userAccount.Accounts != null)
                {
                    foreach (var account in userAccount.Accounts)
                    {
                        account.UserAccountId = userAccount.Id;
                        account.UserAccount = userAccount;
                    }
                }

                _defaultDbContext.UserAccounts.Add(userAccount.ToEntity());
            }

            _defaultDbContext.SaveChanges();
        }


    }
}
