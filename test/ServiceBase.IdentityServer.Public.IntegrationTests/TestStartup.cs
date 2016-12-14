﻿using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Services;
using ServiceBase.Notification.Email;
using System.Linq;

// Entity framework store layer
using ServiceBase.IdentityServer.EntityFramework;

namespace ServiceBase.IdentityServer.UnitTests.Controller.Login
{
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

            // Register mocked email service in case none of the tests registered one already
            if (!services.Any(c => c.ServiceType == typeof(IEmailService)))
            {

                var emailServiceMock = new Mock<IEmailService>();
                services.AddSingleton<IEmailService>(emailServiceMock.Object);
            }

            services.AddTransient<ICrypto, DefaultCrypto>();

            #region Entity Framework Store Layer

            services.AddEntityFrameworkStores((options) =>
            {
                options.SeedExampleData = false;
                options.SeedExampleData = true;
            });

            // Register default store initializer in case none of the tests registered one already
            if (!services.Any(c => c.ServiceType == typeof(IStoreInitializer)))
            {
                services.AddTransient<IStoreInitializer, DefaultStoreInitializer>();
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
            app.InitializeStores();
        }
    }
}