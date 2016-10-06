using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Postgres
{
    public class PersistedGrantService : IPersistedGrantService
    {
        public Task<IEnumerable<Consent>> GetAllGrantsAsync(string subjectId)
        {
            throw new NotImplementedException();
        }

        public Task<AuthorizationCode> GetAuthorizationCodeAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task<Token> GetReferenceTokenAsync(string handle)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
        {
            throw new NotImplementedException();
        }

        public Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllGrantsAsync(string subjectId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAuthorizationCodeAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task RemoveReferenceTokenAsync(string handle)
        {
            throw new NotImplementedException();
        }

        public Task RemoveReferenceTokensAsync(string subjectId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRefreshTokenAsync(string refreshTokenHandle)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRefreshTokensAsync(string subjectId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserConsentAsync(string subjectId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task StoreAuthorizationCodeAsync(string handle, AuthorizationCode code)
        {
            throw new NotImplementedException();
        }

        public Task StoreReferenceTokenAsync(string handle, Token token)
        {
            throw new NotImplementedException();
        }

        public Task StoreRefreshTokenAsync(string handle, RefreshToken refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task StoreUserConsentAsync(Consent consent)
        {
            throw new NotImplementedException();
        }
    }
}
