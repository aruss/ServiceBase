using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ServiceBase.IdentityServer.UnitTests.Controller.Login
{
    // http://stackoverflow.com/questions/30557521/how-to-access-httpcontext-inside-a-unit-test-in-asp-net-5-mvc-6

    [Collection("Login")]
    public class RecoverTests
    {
        string _returnUrl = "/connect/authorize/login?client_id=mvc&amp;redirect_uri=http%3A%2F%2Flocalhost%3A3308%2Fsignin-oidc&amp;response_type=code%20id_token&amp;scope=openid%20profile%20api1&amp;response_mode=form_post&amp;nonce=636169296869162129.MTA5NGU1ZDQtOTA0My00MmY5LWFjZTEtNjAwOWQxNDU5OWFiODJkZTkxMDUtNGQxZC00NzY0LWJmMzAtNWEzMjUwYmE2YzUw&amp;state=CfDJ8McEKbBuVCdHkFjjPyy6vSP1JqZA1Zyajac6fBjwB0NULLiLseWE9RX5i9DI7mlVsn53pxg8RV5UF9bzr9teklpzR2fcc_L2zHOSWxRJFkx9fc02jdlNe9aEO1fLUMlcuq3eZkNk5wE0v8GaMysuBzUDdUEGlJHJjnQvv9HPhEx78Fd4rD_qwDShFfNKQTRoQ8WhiYSL_2-s_0xWRbdMiMdgCSboYXOaa1_bq76mXQh-MLJV-k4Ouq1tshR4N14QTQlhj93C-Gl9Jg6KOIo8X009cvWOhBP2ne0Y5UIVKDdAp-pB6VCXunAdARzZNi-XLEzib6I-BWSis1465tcMT9EqVIEH-TtdRC9rAyD8z03TptD056MyjcWtwqajeFgOLA";
        HttpClient _client;
        TestServer _server;

        public RecoverTests()
        {
            _server = ServerHelper.CreateServer();
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task GetRecover()
        {
            // Act
            var response = await _client.GetAsync("/recover?returnUrl=" + _returnUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
        }
    }
}

