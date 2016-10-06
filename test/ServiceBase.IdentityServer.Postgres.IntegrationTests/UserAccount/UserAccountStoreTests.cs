using ServiceBase.IdentityServer.Models;
using ServiceBase.Xunit;
using System;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace ServiceBase.IdentityServer.Postgres.IntegrationTests
{
    [Collection("NpgsqlFixture")]
    public class UserAccountStoreTests : IDisposable
    {
        NpgsqlFixture db;

        public UserAccountStoreTests(NpgsqlFixture db)
        {
            this.db = db;
            this.db.CreateDatabaseIfNotExists();
            this.db.InsertData("./UserAccount/UserAccountStoreTests.sql");
        }

        public void Dispose()
        {
            this.db.ClearDatabase();
        }

        private PostgresUserAccountStore GetUserStore()
        {
            var options = new PostgresOptions
            {
                ConnectionString = NpgsqlFixture.ConnectionString
            };

            var logger = new NullLogger<PostgresUserAccountStore>();
            var userStore = new PostgresUserAccountStore(options, logger);

            return userStore;
        }

        private void AssertUser(UserAccount userExpected, UserAccount userActual)
        {
            Assert.NotNull(userExpected);
            Assert.NotNull(userActual);

            Assert.Equal(userExpected.Id, userActual.Id);
            Assert.Equal(userExpected.Email, userActual.Email);
            Assert.Equal(userExpected.IsEmailVerified, userActual.IsEmailVerified);
            Assert.Equal(userExpected.PasswordHash, userActual.PasswordHash);
            Assert.Equal(userExpected.FailedLoginCount, userActual.FailedLoginCount);

            Assert.NotNull(userActual.Claims);
            Assert.Equal(userExpected.Claims.Count(), userActual.Claims.Count());
            for (int i = 0; i < userExpected.Claims.Count(); i++)
            {
                var claimExpected = userExpected.Claims.ElementAt(i);
                var claimActual = userActual.Claims.ElementAt(i);

                Assert.Equal(claimExpected.UserId, claimActual.UserId);
                Assert.Equal(claimExpected.Type, claimActual.Type);
                Assert.Equal(claimExpected.Value, claimActual.Value);
                Assert.Equal(claimExpected.ValueType, claimActual.ValueType);
            }

            Assert.NotNull(userActual.Accounts);
            Assert.Equal(userExpected.Accounts.Count(), userActual.Accounts.Count());
            for (int i = 0; i < userExpected.Accounts.Count(); i++)
            {
                var accountExpected = userExpected.Accounts.ElementAt(i);
                var accountActual = userActual.Accounts.ElementAt(i);

                Assert.Equal(accountExpected.UserId, accountActual.UserId);
                Assert.Equal(accountExpected.Email, accountActual.Email);
                Assert.Equal(accountExpected.Provider, accountActual.Provider);
                Assert.Equal(accountExpected.Subject, accountActual.Subject);
            }
        }

        [Theory]
        [JsonData("./UserAccount/UserAccountStoreTests_Default.json")]
        public void LoadByIdAsync(bool exists, UserAccount userExpected)
        {
            var userStore = this.GetUserStore();
            var userActual = userStore.LoadByIdAsync(userExpected.Id).Result;

            if (exists)
            {
                AssertUser(userExpected, userActual);
            }
            else
            {
                Assert.Null(userActual);
            }
        }

        [Theory]
        [JsonData("./UserAccount/UserAccountStoreTests_Default.json")]
        public void LoadByEmailAsync(bool exists, UserAccount userExpected)
        {
            var userStore = this.GetUserStore();
            var userActual = userStore.LoadByEmailAsync(userExpected.Email).Result;

            if (exists)
            {
                AssertUser(userExpected, userActual);
            }
            else
            {
                Assert.Null(userActual);
            }
        }

        [Theory]
        [JsonData("./UserAccount/UserAccountStoreTests_LoadByEmailWithExternalAsync.json")]
        public void LoadByEmailWithExternalAsync(bool exists, string email, UserAccount userExpected)
        {
            var userStore = this.GetUserStore();
            var userActual = userStore.LoadByEmailWithExternalAsync(email).Result;

            if (exists)
            {
                AssertUser(userExpected, userActual);
            }
            else
            {
                Assert.Null(userActual);
            }
        }

        [Theory]
        [JsonData("./UserAccount/UserAccountStoreTests_LoadByExternalProviderAsync.json")]
        public void LoadByExternalProviderAsync(bool exists, string provider, string subject, UserAccount userExpected)
        {
            var userStore = this.GetUserStore();
            var userActual = userStore.LoadByExternalProviderAsync(provider, subject).Result;

            if (exists)
            {
                AssertUser(userExpected, userActual);
            }
            else
            {
                Assert.Null(userActual);
            }
        }

        [Fact]
        public void WriteAsyncNewUser()
        {
            var userStore = this.GetUserStore();

            var userExpected = new UserAccount
            {
                Id = Guid.NewGuid(),
                FailedLoginCount = 0,
                Email = "bill@localhost",
                IsEmailVerified = true,
                PasswordHash = "password",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsLoginAllowed = true,
                Claims = new UserClaim[]
                {
                    new UserClaim
                    {
                        Type = ClaimTypes.Webpage,
                        Value = "http://bill.com",
                        ValueType = ClaimValueTypes.String
                    }
                },
                Accounts = new ExternalAccount[]
                {
                    new ExternalAccount
                    {
                        Email = "bill@visagebook",
                        Provider = "visagebook",
                        Subject = "1234567890"
                    }
                }
            };

            userStore.WriteAsync(userExpected);

            var userActual = userStore.LoadByIdAsync(userExpected.Id).Result;

            AssertUser(userExpected, userActual);
        }

        /*[Fact]
        public void Foo()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Active = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Email = "jim@panse.de",
                EmailVerified = true,
                EmailVerifiedAt = DateTime.UtcNow,
                FailedLoginCount = 0,
                Password = "password",
                Claims = new UserClaim[]
                {
                    new UserClaim
                    {
                        UserId = userId,
                        Type = "name",
                        Value = "Jim",
                        ValueType = null
                    }
                },
                Accounts = new ExternalAccount[]
                {
                    new ExternalAccount
                    {
                        UserId = userId,
                        Provider= "google",
                        Subject = "1234567890",
                        Email= "jim@giggle.com"
                    }
                }
            };

            File.WriteAllText("./user.json", JsonConvert.SerializeObject(user));
        }*/
    }
}
