using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBase.IdentityServer.UnitTests.Controller.Login
{
    public static class ServerHelper
    {
        public static TestServer CreateServer()
        {
            // Arrange
            var contentRoot = Path.Combine(Directory.GetCurrentDirectory(), "src", "ServiceBase.IdentityServer.Public");
            if (!Directory.Exists(contentRoot))
            {
                contentRoot = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "src", "ServiceBase.IdentityServer.Public");
            }

            var server = new TestServer(new WebHostBuilder()
                .ConfigureServices((services) =>
                {

                })

                .UseContentRoot(contentRoot)
                .UseStartup<TestStartup>()
            );





            return server;
        }
    }
}