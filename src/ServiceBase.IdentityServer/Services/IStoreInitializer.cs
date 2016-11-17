using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Services
{
    public interface IStoreInitializer
    {
        Task Initialize();
    }
}
