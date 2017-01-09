using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using ServiceBase.IdentityServer.EntityFramework.DbContexts;
using ServiceBase.IdentityServer.EntityFramework.Mappers;
using ServiceBase.IdentityServer.EntityFramework.Options;
using ServiceBase.IdentityServer.EntityFramework.Stores;
using ServiceBase.IdentityServer.Models;
using ServiceBase.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ServiceBase.IdentityServer.EntityFramework.IntegrationTests.Stores
{
    public class UserAccountStoreTests : IClassFixture<DatabaseProviderFixture<UserAccountDbContext>>
    {
        private static readonly EntityFrameworkOptions StoreOptions = new EntityFrameworkOptions();

        public static readonly TheoryData<DbContextOptions<UserAccountDbContext>> TestDatabaseProviders = new TheoryData<DbContextOptions<UserAccountDbContext>>
        {
            DatabaseProviderBuilder.BuildInMemory<UserAccountDbContext>(nameof(UserAccountStoreTests), StoreOptions),
            DatabaseProviderBuilder.BuildSqlite<UserAccountDbContext>(nameof(UserAccountStoreTests), StoreOptions),
            DatabaseProviderBuilder.BuildSqlServer<UserAccountDbContext>(nameof(UserAccountStoreTests), StoreOptions)
        };

        public UserAccountStoreTests(DatabaseProviderFixture<UserAccountDbContext> fixture)
        {
            fixture.Options = TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<UserAccountDbContext>)y)).ToList();
            fixture.StoreOptions = StoreOptions;
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void LoadByIdAsync_WhenUserAccountExists_ExpectUserAccountRetured(DbContextOptions<UserAccountDbContext> options)
        {
            var testUserAccount = new UserAccount
            {
                Id = Guid.NewGuid(),
                Email = "jim@panse.de"
            };

            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                context.UserAccounts.Add(testUserAccount.ToEntity());
                context.SaveChanges();
            }

            UserAccount userAccount;
            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                var store = new UserAccountStore(context, NullLogger<UserAccountStore>.Create());

                userAccount = store.LoadByIdAsync(testUserAccount.Id).Result;
            }

            Assert.NotNull(userAccount);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void LoadByEmailAsync_WhenUserAccountExists_ExpectUserAccountRetured(DbContextOptions<UserAccountDbContext> options)
        {
            var testUserAccount = new UserAccount
            {
                Email = "jim2@panse.de"
            };

            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                context.UserAccounts.Add(testUserAccount.ToEntity());
                context.SaveChanges();
            }

            UserAccount userAccount;
            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                var store = new UserAccountStore(context, NullLogger<UserAccountStore>.Create());

                userAccount = store.LoadByEmailAsync(testUserAccount.Email).Result;
            }

            Assert.NotNull(userAccount);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void LoadByEmailWithExternalAsync_WhenUserAccountExists_ExpectUserAccountRetured(DbContextOptions<UserAccountDbContext> options)
        {
            var testUserAccount = new UserAccount
            {
                Email = "jim2@panse.de",
                Accounts = new List<ExternalAccount>
                {
                    new ExternalAccount
                    {
                        Provider ="provider",
                        Email = "foo@provider.com",
                        Subject = "123456789"
                    },
                    new ExternalAccount
                    {
                        Provider = "provider",
                        Email = "bar@anotherprovider.com",
                        Subject = "asda5sd4a564da6"
                    }
                }
            };

            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                context.UserAccounts.Add(testUserAccount.ToEntity());
                context.SaveChanges();
            }

            UserAccount userAccount;
            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                var store = new UserAccountStore(context, NullLogger<UserAccountStore>.Create());

                userAccount = store.LoadByEmailWithExternalAsync(testUserAccount.Email).Result;
            }

            Assert.NotNull(userAccount);
        }


        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void LoadByVerificationKeyAsync_WhenUserAccountExists_ExpectUserAccountRetured(DbContextOptions<UserAccountDbContext> options)
        {
            var testUserAccount = new UserAccount
            {
                Email = "jim3@panse.de",
                VerificationKey = Guid.NewGuid().ToString()
            };

            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                context.UserAccounts.Add(testUserAccount.ToEntity());
                context.SaveChanges();
            }

            UserAccount userAccount;
            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                var store = new UserAccountStore(context, NullLogger<UserAccountStore>.Create());

                userAccount = store.LoadByVerificationKeyAsync(testUserAccount.VerificationKey).Result;
            }

            Assert.NotNull(userAccount);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void LoadByExternalProviderAsync_WhenUserAccountExists_ExpectUserAccountRetured(DbContextOptions<UserAccountDbContext> options)
        {
            var testExternalAccount = new ExternalAccount
            {
                Email = "jim4@panse.de",
                Provider = "facebook",
                Subject = "123456789"
            };

            var testUserAccount = new UserAccount
            {
                Email = "jim4@panse.de",
                Accounts = new List<ExternalAccount>
                {
                    testExternalAccount
                }
            };

            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                context.UserAccounts.Add(testUserAccount.ToEntity());
                context.SaveChanges();
            }

            UserAccount userAccount;
            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                var store = new UserAccountStore(context, NullLogger<UserAccountStore>.Create());

                userAccount = store.LoadByExternalProviderAsync(
                    testExternalAccount.Provider,
                    testExternalAccount.Subject).Result;
            }

            Assert.NotNull(userAccount);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void WriteAsync_WhenUserAccountExists_ExpectUserAccountRetured(DbContextOptions<UserAccountDbContext> options)
        {
            var testUserAccount1 = new UserAccount
            {
                Email = "foo@localhost",
                Accounts = new List<ExternalAccount>
                {
                    new ExternalAccount
                    {
                        Email = "foo@provider",
                        Provider = "facebook",
                        Subject = "123456789",
                    },
                    new ExternalAccount
                    {
                        Email = "bar@provider",
                        Provider = "google",
                        Subject = "789456123",
                    }
                },
                Claims = new List<UserAccountClaim>
                {
                    new UserAccountClaim("name", "foo"),
                    new UserAccountClaim("email", "foo@localhost"),
                }
            };

            var testUserAccount2 = new UserAccount
            {
                Email = "bar@localhost",
                Accounts = new List<ExternalAccount>
                {
                    new ExternalAccount
                    {
                        Email = "baz@provider",
                        Provider = "facebook",
                        Subject = "465462131",
                    },
                    new ExternalAccount
                    {
                        Email = "butz@provider",
                        Provider = "hotmail",
                        Subject = "798756136",
                    }
                },
                Claims = new List<UserAccountClaim>
                {
                    new UserAccountClaim("name", "bar"),
                    new UserAccountClaim("email", "bar@localhost"),
                }
            };

            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                context.UserAccounts.Add(testUserAccount1.ToEntity());
                context.SaveChanges();
            }

            UserAccount userAccount;
            using (var context = new UserAccountDbContext(options, StoreOptions))
            {
                var store = new UserAccountStore(context, NullLogger<UserAccountStore>.Create());

                userAccount = store.WriteAsync(testUserAccount2).Result;
            }

            Assert.NotNull(userAccount);
        }
    }
}