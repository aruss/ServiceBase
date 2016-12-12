using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBase.IdentityServer.UnitTests.Controller.Login
{
    [Collection("Login")]
    public class GeneralTests
    {
        HttpClient _client;

        public GeneralTests()
        {
            // Arrange
            var contentRoot = Path.Combine(Directory.GetCurrentDirectory(), "src", "ServiceBase.IdentityServer.Public");
            if (!Directory.Exists(contentRoot))
            {
                contentRoot = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "src", "ServiceBase.IdentityServer.Public");
            }

            var server = new TestServer(new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .UseStartup<TestStartup>()
            );

            _client = server.CreateClient();
        }

        [Fact]
        public async Task GetIndex()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
        }
    }
}

