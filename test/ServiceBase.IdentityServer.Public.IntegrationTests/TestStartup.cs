using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.EntityFramework;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Services;
using ServiceBase.Notification.Email;
using System.Linq;

namespace ServiceBase.IdentityServer.Public.IntegrationTests
{
    public class TestStartup
    {
        public TestStartup(IHostingEnvironment environment)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

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

            // Register mocked email service in case none of the tests registered one already
            if (!services.Any(c => c.ServiceType == typeof(IEmailService)))
            {

                var emailServiceMock = new Mock<IEmailService>();
                services.AddSingleton<IEmailService>(emailServiceMock.Object);
            }

            services.AddTransient<ICrypto, DefaultCrypto>();
            services.AddTransient<ServiceBase.Events.IEventService, ServiceBase.IdentityServer.Events.DefaultEventService>();

            #region Entity Framework Store Layer

            services.AddEntityFrameworkStores((options) =>
            {
                options.MigrateDatabase = false;
                options.SeedExampleData = false;
            });

            // Register default store initializer in case none of the tests registered one already
            if (!services.Any(c => c.ServiceType == typeof(IStoreInitializer)))
            {
                services.AddTransient<IStoreInitializer, TestStoreInitializer>();
            }

            #endregion

            services
              .AddMvc()
              .AddRazorOptions(razor =>
              {
                  razor.ViewLocationExpanders.Add(new Public.UI.CustomViewLocationExpander());
              });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<RequestIdMiddleware>();
            app.UseExceptionHandler("/error");
            app.UseIdentityServer();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.InitializeStores();
        }
    }
}