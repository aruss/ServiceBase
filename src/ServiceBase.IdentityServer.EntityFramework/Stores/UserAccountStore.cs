using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceBase.IdentityServer.EntityFramework.DbContexts;
using ServiceBase.IdentityServer.EntityFramework.Mappers;
using ServiceBase.IdentityServer.Models;
using ServiceBase.IdentityServer.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.EntityFramework
{
    // TODO: make use of value type  System.Security.Claims.ClaimValueTypes while create UserClaim
    // http://www.npgsql.org/doc/faq.html

    public class UserAccountStore : IUserAccountStore
    {
        private readonly UserAccountDbContext _context;
        private readonly ILogger<UserAccountStore> _logger;

        public UserAccountStore(UserAccountDbContext context, ILogger<UserAccountStore> logger)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
            _logger = logger;
        }

        public Task WriteExternalAccountAsync(ExternalAccount externalAccount)
        {
            var userAccountId = externalAccount.UserAccount != null ? externalAccount.UserAccount.Id : externalAccount.UserAccountId;
            var existingUserAccount = _context.UserAccounts.SingleOrDefault(x => x.Id == userAccountId);

            if (existingUserAccount == null)
            {
                _logger.LogError("{existingUserAccountId} not found in database", userAccountId);
                return Task.FromResult(0);
            }

            var existingExternalAccount = _context.ExternalAccounts.SingleOrDefault(x =>
                x.Provider == externalAccount.Provider && x.Subject == externalAccount.Subject);

            if (existingExternalAccount == null)
            {
                _logger.LogDebug("{0} {1} not found in database", externalAccount.Provider, externalAccount.Subject);

                var persistedExternalAccount = externalAccount.ToEntity();
                _context.ExternalAccounts.Add(persistedExternalAccount);
            }
            else
            {
                _logger.LogDebug("{0} {1} found in database", existingExternalAccount.Provider, existingExternalAccount.Subject);

                externalAccount.UpdateEntity(existingExternalAccount);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Exception storing external account");
            }

            return Task.FromResult(0);
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            var userAccount = _context.UserAccounts
             //.Include(x => x.Accounts)
             //.Include(x => x.Claims)
             .FirstOrDefault(x => x.Id == id);

            if (userAccount != null)
            {
                _context.Remove(userAccount);
                await _context.SaveChangesAsync();
            }
        }

        public Task DeleteExternalAccountAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<UserAccount> LoadByEmailAsync(string email)
        {
            var userAccount = _context.UserAccounts
                .Include(x => x.Accounts)
                .Include(x => x.Claims)
                .FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            var model = userAccount?.ToModel();

            _logger.LogDebug("{email} found in database: {userAccountFound}", email, model != null);

            return Task.FromResult(model);
        }

        public Task<UserAccount> LoadByEmailWithExternalAsync(string email)
        {
            var userAccount = _context.UserAccounts
                .Include(x => x.Accounts)
                .Include(x => x.Claims)
                .FirstOrDefault(x =>
                    x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) ||
                    x.Accounts.Any(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

            var model = userAccount?.ToModel();

            _logger.LogDebug("{email} found in database: {userAccountFound}", email, model != null);

            return Task.FromResult(model);
        }

        public Task<UserAccount> LoadByExternalProviderAsync(string provider, string subject)
        {
            var userAccount = _context.UserAccounts
                .Include(x => x.Accounts)
                .Include(x => x.Claims)
                .FirstOrDefault(x =>
                    x.Accounts.Any(c => c.Provider.Equals(provider, StringComparison.OrdinalIgnoreCase)) ||
                    x.Accounts.Any(c => c.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase)));

            var model = userAccount?.ToModel();

            _logger.LogDebug("{provider}, {subject} found in database: {userAccountFound}", provider, subject, model != null);

            return Task.FromResult(model);
        }

        public Task<UserAccount> LoadByIdAsync(Guid id)
        {
            var userAccount = _context.UserAccounts
               .Include(x => x.Accounts)
               .Include(x => x.Claims)
               .FirstOrDefault(x => x.Id == id);

            var model = userAccount?.ToModel();

            _logger.LogDebug("{id} found in database: {userAccountFound}", id, model != null);

            return Task.FromResult(model);
        }

        public Task<UserAccount> LoadByVerificationKeyAsync(string key)
        {
            var userAccount = _context.UserAccounts
                .Include(x => x.Accounts)
                .Include(x => x.Claims)
                .FirstOrDefault(x => x.VerificationKey == key);

            var model = userAccount?.ToModel();

            _logger.LogDebug("{key} found in database: {userAccountFound}", key, model != null);

            return Task.FromResult(model);
        }

        public Task WriteAsync(UserAccount userAccount)
        {
            if (userAccount == null) throw new ArgumentNullException(nameof(userAccount));

            var existing = _context.UserAccounts.SingleOrDefault(x => x.Id == userAccount.Id);
            if (existing == null)
            {
                _logger.LogDebug("{userAccountId} not found in database", userAccount.Id);

                var entity = userAccount.ToEntity();
                _context.UserAccounts.Add(entity);
            }
            else
            {
                _logger.LogDebug("{userAccountId} found in database", userAccount.Id);

                userAccount.UpdateEntity(existing);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Exception storing user account");
            }

            return Task.FromResult(0);
        }
    }
}