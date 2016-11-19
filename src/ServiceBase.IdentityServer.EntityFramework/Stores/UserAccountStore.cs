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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Task<UserAccount> LoadByVerificationKeyAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<UserAccount> UpdateAsync(UserAccount userAccount)
        {
            throw new NotImplementedException();
        }

        public Task<UserAccount> WriteAsync(UserAccount userAccount)
        {
            throw new NotImplementedException();
        }
    }
}