using IdentityServer4.EntityFramework.Mappers;
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
        private readonly DefaultDbContext _context;
        private readonly ILogger<UserAccountStore> _logger;

        public UserAccountStore(DefaultDbContext context, ILogger<UserAccountStore> logger)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
            _logger = logger;
        }
        
        public Task<ExternalAccount> AddExternalAccountAsync(Guid userAccoutId, ExternalAccount externalAccount)
        {
            if (userAccoutId == Guid.Empty) throw new ArgumentException(nameof(userAccoutId));
            if (externalAccount == null) throw new ArgumentNullException(nameof(externalAccount));

            var now = DateTime.UtcNow;
            var entity = externalAccount.ToEntity();
            entity.UserId = userAccoutId;
            entity.CreatedAt = now;
            var entry = _context.ExternalAccounts.Add(entity);

            _context.SaveChanges();

            var model = entry.Entity.ToModel();
            return Task.FromResult(model);
        }

        public Task DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteExternalAccountAsync(ExternalAccount externalAccount)
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

        public Task<UserAccount> UpdateAsync(UserAccount userAccount)
        {
            if (userAccount == null) throw new ArgumentNullException(nameof(userAccount));

            var entity = userAccount.ToEntity();

            var now = DateTime.UtcNow;
            entity.UpdatedAt = now;

            var entry = _context.UserAccounts.Update(entity);

            _context.SaveChanges();

            var model = entry.Entity.ToModel();
            return Task.FromResult(model);
        }

        public Task<UserAccount> WriteAsync(UserAccount userAccount)
        {
            if (userAccount == null) throw new ArgumentNullException(nameof(userAccount));

            var entity = userAccount.ToEntity();
          
            var now = DateTime.UtcNow;
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            entity.CreatedAt = now;
            entity.UpdatedAt = now;

            if (entity.Accounts != null)
            {
                foreach (var account in entity.Accounts)
                {
                    account.UserId = entity.Id;
                    account.CreatedAt = now;
                }
            }

            if (entity.Claims != null)
            {
                foreach (var claim in entity.Claims)
                {
                    claim.UserId = entity.Id;
                }
            }

            var entry = _context.UserAccounts.Add(entity);

            _context.SaveChanges();

            var model = entry.Entity.ToModel();
            return Task.FromResult(model);
        }
    }
}