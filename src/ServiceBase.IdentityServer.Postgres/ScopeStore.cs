using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Postgres
{
    public class ScopeStore : IScopeStore
    {
        public async Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            throw new NotImplementedException();
        }
    }
}
