using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Postgres
{
    public class ClientStore : IClientStore
    {
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}
