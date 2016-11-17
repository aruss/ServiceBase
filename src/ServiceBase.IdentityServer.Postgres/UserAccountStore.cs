using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using ServiceBase.IdentityServer.Models;
using ServiceBase.IdentityServer.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace ServiceBase.IdentityServer.Postgres
{
    // TODO: make use of value type  System.Security.Claims.ClaimValueTypes while create UserClaim
    // http://www.npgsql.org/doc/faq.html

    public class UserAccountStore : IUserAccountStore
    {
        private PostgresOptions _options;
        private ILogger<UserAccountStore> _logger;

        public UserAccountStore(
            IOptions<PostgresOptions> options,
            ILogger<UserAccountStore> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        private UserAccount ReadUserAccount(GridReader reader)
        {
            var userAccount = reader.Read<UserAccount>().SingleOrDefault();

            if (userAccount != null)
            {
                userAccount.Claims = reader.Read<UserClaim>().ToArray();
                userAccount.Accounts = reader.Read<ExternalAccount>().ToArray();
            }

            return userAccount;
        }

        public async Task<UserAccount> LoadByIdAsync(Guid userId)
        {
            using (var con = new NpgsqlConnection(_options.ConnectionString))
            {
                con.Open();

                var dParams = new DynamicParameters();
                dParams.Add("UserId", userId);

                // userid, type, value, coalesce(valuetype, 'http://www.w3.org/2001/XMLSchema#boolean') as valuetype
                using (var reader = con.QueryMultiple(
                    "SELECT * FROM useraccounts WHERE id = @UserId;" +
                    "SELECT * FROM userclaims WHERE userid = @UserId;" +
                    "SELECT * FROM externalaccounts WHERE userid = @UserId", dParams))
                {
                    return await Task.FromResult(ReadUserAccount(reader));
                }
            }
        }

        public async Task<UserAccount> LoadByEmailAsync(string email)
        {
            if (String.IsNullOrWhiteSpace(email)) throw new ArgumentNullException("email");

            using (var con = new NpgsqlConnection(_options.ConnectionString))
            {
                con.Open();

                var dParams = new DynamicParameters();
                dParams.Add("Email", email.ToLowerInvariant());

                using (var reader = await con.QueryMultipleAsync(
                    "SELECT * FROM useraccounts WHERE email = @Email;" +
                    "SELECT c.* FROM useraccounts u RIGHT JOIN userclaims c ON c.userid = u.id WHERE u.email = @Email;" +
                    "SELECT a.* FROM useraccounts u RIGHT JOIN externalaccounts a ON a.userid = u.id WHERE u.email = @Email;", dParams))
                {
                    return await Task.FromResult(ReadUserAccount(reader));
                }
            }
        }

        public async Task<UserAccount> LoadByExternalProviderAsync(string provider, string subject)
        {
            if (String.IsNullOrWhiteSpace(provider)) throw new ArgumentNullException("provider");
            if (String.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException("subject");

            using (var con = new NpgsqlConnection(_options.ConnectionString))
            {
                con.Open();

                var dParams = new DynamicParameters();
                dParams.Add("Provider", provider.ToLower());
                dParams.Add("Subject", subject);

                using (var reader = await con.QueryMultipleAsync(
                    "SELECT u.* FROM useraccounts u LEFT JOIN externalaccounts a ON a.userid = u.id WHERE a.provider = @Provider AND a.subject = @Subject;" +
                    "SELECT c.* FROM externalaccounts a LEFT JOIN userclaims c ON c.userid = a.userid WHERE a.provider = @Provider AND a.subject = @Subject;" +
                    "SELECT * FROM externalaccounts WHERE userid IN (SELECT userid FROM externalaccounts WHERE provider = @Provider AND subject = @Subject);", dParams))
                {
                    return await Task.FromResult(ReadUserAccount(reader));
                }
            }
        }

        public async Task<UserAccount> LoadByEmailWithExternalAsync(string email)
        {
            if (String.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

            using (var con = new NpgsqlConnection(_options.ConnectionString))
            {
                con.Open();

                var dParams = new DynamicParameters();
                dParams.Add("Email", email.ToLower());

                using (var reader = await con.QueryMultipleAsync(
                    "SELECT u.* FROM useraccounts u LEFT JOIN externalaccounts a ON u.id = a.userid WHERE a.email = @Email OR u.email = @Email GROUP BY u.id;" +
                    "SELECT * FROM userclaims WHERE userid IN(SELECT u.id FROM useraccounts u LEFT JOIN externalaccounts a ON u.id = a.userid WHERE a.email = @Email OR u.email = @Email);" +
                    "SELECT * FROM externalaccounts WHERE userid IN (SELECT u.id FROM useraccounts u LEFT JOIN externalaccounts a ON u.id = a.userid WHERE a.email = @Email OR u.email = @Email);", dParams))
                {
                    return await Task.FromResult(ReadUserAccount(reader));
                }
            }
        }

        public async Task<UserAccount> WriteAsync(UserAccount userAccount)
        {
            if (userAccount == null) throw new ArgumentNullException(nameof(userAccount));

            using (var con = new NpgsqlConnection(_options.ConnectionString))
            {
                con.Open();

                using (var scope = con.BeginTransaction())
                {
                    var now = DateTime.UtcNow;
                    userAccount.Id = userAccount.Id == Guid.Empty ? Guid.NewGuid() : userAccount.Id;
                    userAccount.CreatedAt = now;
                    userAccount.UpdatedAt = now;

                    con.Execute(
                        "INSERT INTO useraccounts(id, email, isemailverified, emailverifiedat, isloginallowed, lastloginat, lastfailedloginat, failedlogincount, passwordhash, passwordchangedat, verificationkey, verificationpurpose, verificationkeysentat, verificationstorage, createdat, updatedat) VALUES " +
                        "(@Id, @Email, @IsEmailVerified, @EmailVerifiedAt, @IsLoginAllowed, @LastLoginAt, @LastFailedLoginAt, @FailedLoginCount, @PasswordHash, @PasswordChangedAt, @VerificationKey, @VerificationPurpose, @VerificationKeySentAt, @VerificationStorage, @CreatedAt, @UpdatedAt);", userAccount);

                    if (userAccount.Accounts != null)
                    {
                        foreach (var account in userAccount.Accounts)
                        {
                            account.UserId = userAccount.Id;
                            account.CreatedAt = now;
                        }

                        con.Execute(
                            "INSERT INTO externalaccounts(userid, provider, subject, email, lastloginat, createdat) VALUES " +
                            "(@UserId, @Provider, @Subject, @Email, @LastLoginAt, @CreatedAt);", userAccount.Accounts);
                    }

                    if (userAccount.Claims != null)
                    {
                        foreach (var claim in userAccount.Claims)
                        {
                            claim.UserId = userAccount.Id;
                        }

                        con.Execute(
                              "INSERT INTO userclaims(userid, type, value, valuetype) VALUES " +
                              "(@UserId, @Type, @Value, @ValueType);", userAccount.Claims);
                    }

                    scope.Commit();
                }
            }

            // TODO: clone the object 
            return userAccount;
        }

        public async Task<UserAccount> UpdateAsync(UserAccount userAccount)
        {
            if (userAccount == null) throw new ArgumentNullException(nameof(userAccount));

            // throw new NotImplementedException(); 
            using (var con = new NpgsqlConnection(_options.ConnectionString))
            {
                con.Open();

                using (var scope = con.BeginTransaction())
                {
                    var now = DateTime.UtcNow;
                    userAccount.UpdatedAt = now;

                    con.Execute(
                        "UPDATE useraccounts SET id=@Id, email=@Email, isemailverified=@IsEmailVerified, emailverifiedat=@EmailVerifiedAt, isloginallowed=@IsLoginAllowed, lastloginat=@LastLoginAt, lastfailedloginat=@LastFailedLoginAt, failedlogincount=@FailedLoginCount, passwordhash=@PasswordHash, " +
                        "passwordchangedat=@PasswordChangedAt, verificationkey=@VerificationKey, verificationpurpose=@VerificationPurpose, verificationkeysentat=@VerificationKeySentAt, verificationstorage=@VerificationStorage, createdat=@CreatedAt, updatedat=@UpdatedAt " +
                        "WHERE id = @Id;", userAccount);

                    // Update claims and accounts

                    scope.Commit();
                }
            }

            return await Task.FromResult(userAccount);
        }

        public async Task<UserAccount> LoadByVerificationKeyAsync(string key)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            using (var con = new NpgsqlConnection(_options.ConnectionString))
            {
                con.Open();

                var dParams = new DynamicParameters();
                dParams.Add("VerificationKey", key);

                using (var reader = await con.QueryMultipleAsync(
                    "SELECT * FROM useraccounts u WHERE u.verificationkey = @VerificationKey;" +
                    "SELECT c.* FROM useraccounts u LEFT JOIN userclaims c ON u.id = c.userid WHERE u.verificationkey = @VerificationKey;" +
                    "SELECT a.* FROM useraccounts u LEFT JOIN externalaccounts a ON u.id = a.userid WHERE u.verificationkey = @VerificationKey;", dParams))
                {
                    return await Task.FromResult(ReadUserAccount(reader));
                }
            }
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ExternalAccount> AddExternalAccountAsync(Guid userAccoutId, ExternalAccount externalAccount)
        {
            if (externalAccount == null) throw new ArgumentNullException(nameof(externalAccount));

            using (var con = new NpgsqlConnection(_options.ConnectionString))
            {
                con.Open();

                using (var scope = con.BeginTransaction())
                {
                    var now = DateTime.UtcNow;

                    externalAccount.UserId = userAccoutId;
                    externalAccount.CreatedAt = now;

                    con.Execute(
                        "INSERT INTO externalaccounts(userid, provider, subject, email, lastloginat, createdat) VALUES " +
                        "(@UserId, @Provider, @Subject, @Email, @LastLoginAt, @CreatedAt);", externalAccount);

                    scope.Commit();
                }
            }

            return externalAccount;
        }

        public Task DeleteExternalAccountAsync(ExternalAccount externalAccount)
        {
            throw new NotImplementedException();
        }
    }
}