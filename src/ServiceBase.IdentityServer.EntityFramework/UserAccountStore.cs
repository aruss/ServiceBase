using ServiceBase.IdentityServer.Models;
using ServiceBase.IdentityServer.Services;
using System;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.EntityFramework
{
    // TODO: make use of value type  System.Security.Claims.ClaimValueTypes while create UserClaim
    // http://www.npgsql.org/doc/faq.html

    public class UserAccountStore : IUserAccountStore
    {
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
            throw new NotImplementedException();
        }

        public Task<UserAccount> LoadByEmailWithExternalAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<UserAccount> LoadByExternalProviderAsync(string provider, string subject)
        {
            throw new NotImplementedException();
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