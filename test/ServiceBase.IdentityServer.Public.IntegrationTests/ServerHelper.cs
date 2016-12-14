using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace ServiceBase.IdentityServer.UnitTests.Controller.Login
{
    public static class ServerHelper
    {
        public static TestServer CreateServer(Action<IServiceCollection> configureServices = null)
        {
            // Arrange
            var contentRoot = Path.Combine(Directory.GetCurrentDirectory(), "src", "ServiceBase.IdentityServer.Public");
            if (!Directory.Exists(contentRoot))
            {
                contentRoot = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "src", "ServiceBase.IdentityServer.Public");
            }

            var builder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .UseStartup<TestStartup>();

            if (configureServices != null)
            {
                builder = builder.ConfigureServices(configureServices);
            }

            var server = new TestServer(builder);

            return server;
        }
    }
}