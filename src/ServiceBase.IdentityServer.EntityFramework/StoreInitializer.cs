using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBase.IdentityServer.Services;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.EntityFramework
{
    public class StoreInitializer : IStoreInitializer
    {
        private EntityFrameworkOptions _options;
        private ILogger<StoreInitializer> _logger;
        private IHostingEnvironment _env;

        public StoreInitializer(EntityFrameworkOptions options, ILogger<StoreInitializer> logger, IHostingEnvironment env)
        {
            _options = options;
            _logger = logger;
            _env = env;
        }

        public async Task Initialize()
        {
          
            // seed db here 

        }

        public class ExistsResult
        {
            public bool Exists { get; set; }
        }        
    }
}


