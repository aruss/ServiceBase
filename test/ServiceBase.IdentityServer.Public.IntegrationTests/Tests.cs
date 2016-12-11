using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceBase.Config;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.EntityFramework;
using ServiceBase.IdentityServer.Services;
using ServiceBase.Notification.Email;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace ServiceBase.IdentityServer.UnitTests.Controller.Login
{
    // http://stackoverflow.com/questions/30557521/how-to-access-httpcontext-inside-a-unit-test-in-asp-net-5-mvc-6

    [Collection("Login")]
    public class LoggingControllerTests
    {
        HttpClient _client;
        string _returnUrl = "/connect/authorize/login?client_id=mvc&amp;redirect_uri=http%3A%2F%2Flocalhost%3A3308%2Fsignin-oidc&amp;response_type=code%20id_token&amp;scope=openid%20profile%20api1&amp;response_mode=form_post&amp;nonce=636169296869162129.MTA5NGU1ZDQtOTA0My00MmY5LWFjZTEtNjAwOWQxNDU5OWFiODJkZTkxMDUtNGQxZC00NzY0LWJmMzAtNWEzMjUwYmE2YzUw&amp;state=CfDJ8McEKbBuVCdHkFjjPyy6vSP1JqZA1Zyajac6fBjwB0NULLiLseWE9RX5i9DI7mlVsn53pxg8RV5UF9bzr9teklpzR2fcc_L2zHOSWxRJFkx9fc02jdlNe9aEO1fLUMlcuq3eZkNk5wE0v8GaMysuBzUDdUEGlJHJjnQvv9HPhEx78Fd4rD_qwDShFfNKQTRoQ8WhiYSL_2-s_0xWRbdMiMdgCSboYXOaa1_bq76mXQh-MLJV-k4Ouq1tshR4N14QTQlhj93C-Gl9Jg6KOIo8X009cvWOhBP2ne0Y5UIVKDdAp-pB6VCXunAdARzZNi-XLEzib6I-BWSis1465tcMT9EqVIEH-TtdRC9rAyD8z03TptD056MyjcWtwqajeFgOLA";
        string _returnUrlEncoded = "%2Fconnect%2Fauthorize%2Flogin%3Fclient_id%3Dmvc%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A3308%252Fsignin-oidc%26response_type%3Dcode%2520id_token%26scope%3Dopenid%2520profile%2520api1%26response_mode%3Dform_post%26nonce%3D636167224425361654.MWQyYzcwZTktMGRiYy00YTc0LWI3MTgtMThlMmJiYjQxNTVmZDMyMGI5NjQtOGM4ZS00NjAzLTg5OGYtMTM0ZWYwNjliYWZm%26state%3DCfDJ8McEKbBuVCdHkFjjPyy6vSNHOPb1BvOsBsSu_px4X6gkT_fbm1EmIE-fvTt1RTiHQiK6lVOS9S_AUj5qmkP210O08RGfQd9vsQ2gyMF4TTUVd1dVXedJpzIkpF2rIHs2xG9tbCmpvBHjb-Ln2Fl2ISvwBnKmAQGTRTtbJh75x7WJAFk8MOX_o7MtckUDAX7RxDmlDT09NyjlOkaLl8dnRADP6IABMw5hSU2sQk7G35CI8_kcecHxEKsYscIlDuxuArZTIwWPV8VRMw2fv7ueFZ__az3d3GhrK2nbGhM2EIFlSy6uK4-HIfmbWqU2IaOzWUuUPwKghl0frnD2ANRuqEZO4LNHvkZltST6jgNGACYpFvZPumqAIZoGUWrJ29SMeQ";

        public LoggingControllerTests()
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

        [Fact]
        public async Task GetLogin()
        {
            // Act
            var response = await _client.GetAsync("/login?returnUrl=%2Fconnect%2Fauthorize%2Flogin%3Fclient_id%3Dmvc%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A3308%252Fsignin-oidc%26response_type%3Dcode%2520id_token%26scope%3Dopenid%2520profile%2520api1%26response_mode%3Dform_post%26nonce%3D636167224425361654.MWQyYzcwZTktMGRiYy00YTc0LWI3MTgtMThlMmJiYjQxNTVmZDMyMGI5NjQtOGM4ZS00NjAzLTg5OGYtMTM0ZWYwNjliYWZm%26state%3DCfDJ8McEKbBuVCdHkFjjPyy6vSNHOPb1BvOsBsSu_px4X6gkT_fbm1EmIE-fvTt1RTiHQiK6lVOS9S_AUj5qmkP210O08RGfQd9vsQ2gyMF4TTUVd1dVXedJpzIkpF2rIHs2xG9tbCmpvBHjb-Ln2Fl2ISvwBnKmAQGTRTtbJh75x7WJAFk8MOX_o7MtckUDAX7RxDmlDT09NyjlOkaLl8dnRADP6IABMw5hSU2sQk7G35CI8_kcecHxEKsYscIlDuxuArZTIwWPV8VRMw2fv7ueFZ__az3d3GhrK2nbGhM2EIFlSy6uK4-HIfmbWqU2IaOzWUuUPwKghl0frnD2ANRuqEZO4LNHvkZltST6jgNGACYpFvZPumqAIZoGUWrJ29SMeQ");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task GetLoginWithIdp()
        {
            // Act
            var response = await _client.GetAsync("/login?returnUrl=%2Fconnect%2Fauthorize%2Flogin%3Fclient_id%3Dmvc%26idp%3Dfacebook%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A3308%252Fsignin-oidc%26response_type%3Dcode%2520id_token%26scope%3Dopenid%2520profile%2520api1%26response_mode%3Dform_post%26nonce%3D636169311618127555.YjlhZWU0MDAtM2IwYi00ZjMwLWE5MzktNjY5ZTNkM2M2YzcxOWVhMzM5OTEtMzY0MC00MDE1LWI0ZTctMWViMmY2NzE3YWE5%26state%3DCfDJ8McEKbBuVCdHkFjjPyy6vSP1HA6e39XLaF97kFrcwltEi7yCJ8Iwu5Z2lFDkWCWErNJIca4xEQZy_shO-fYncxZgTZYH7axkcJY9U0l7BlFASUVaQ5hxzhs5-dEik8lMjjknSs7VSXNHMAuDSTzDN1BDU5VvJKfxeOpeXGI9lcqyaHkTP0kVM3pEjoap0PfD2yGD1gxgYIJTbSUkORODTy643aIDoy5pZ1Yk94PLAQF5sSZSYCG4X7ccdPC03IsB0v6chhu6rW5dnzfNAqt_MhCAbeg2RFoFkODYlTI-LMePXORYPUP-BIA01i3j01HWUAqz_U60DLISnIP8XGSdAtjWHlHwqIm_7u8h7CpcSfoi-M2FS34MkjBQaUGHZQx6hA");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
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


        [Fact]
        public async Task GetRegister()
        {
            // Act
            var response = await _client.GetAsync("/register?returnUrl=" + _returnUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task GetLogout()
        {
            // Act
            var response = await _client.GetAsync("/logout?returnUrl=" + _returnUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
        }


        [Fact]
        public async Task PostLogin()
        {
            // Act
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "alice@localhost"),
                new KeyValuePair<string, string>("Password", "alice@localhost"),
                new KeyValuePair<string, string>("RememberLogin", "true"),
                new KeyValuePair<string, string>("ReturnUrl", _returnUrl)
            });

            var response = await _client.PostAsync("/", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
        }
    }

    public class TestStartup
    {
        public TestStartup(IHostingEnvironment environment)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<ApplicationOptions>((option) =>
            {

            });

            var builder = services.AddIdentityServer((options) =>
            {
                //options.RequireSsl = false;
                options.EventsOptions = new EventsOptions
                {
                    RaiseErrorEvents = true,
                    RaiseFailureEvents = true,
                    RaiseInformationEvents = true,
                    RaiseSuccessEvents = true
                };
                options.UserInteractionOptions.LoginUrl = "/login";
                options.UserInteractionOptions.LogoutUrl = "/logout";
                options.UserInteractionOptions.ConsentUrl = "/consent";
                options.UserInteractionOptions.ErrorUrl = "/error";
                options.AuthenticationOptions.FederatedSignOutPaths.Add("/signout-oidc");
            })
                .AddTemporarySigningCredential()
                .AddProfileService<ProfileService>()
                .AddSecretParser<ClientAssertionSecretParser>()
                .AddSecretValidator<PrivateKeyJwtSecretValidator>()
                .AddTemporarySigningCredential();

            var emailServiceMock = new Mock<IEmailService>();
            services.AddSingleton<IEmailService>(emailServiceMock.Object);

            services.AddTransient<ICrypto, DefaultCrypto>();

            services.AddEntityFrameworkStores((option) =>
            {
                       option.SeedExampleData = true;
            });

            services
              .AddMvc()
              .AddRazorOptions(razor =>
              {
                  razor.ViewLocationExpanders.Add(new Public.UI.CustomViewLocationExpander());
              });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseIdentityServer();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<IStoreInitializer>().InitializeStores();
            }
        }
    }
}

