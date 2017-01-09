using IdentityServer4;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceBase.Configuration;
using ServiceBase.IdentityServer.Configuration;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Services;
using ServiceBase.Notification.Email;
using System;
using System.IO;
using ServiceBase.IdentityServer.EntityFramework;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;

namespace ServiceBase.IdentityServer.Public
{

    public class Startup
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _environment;
        private readonly IConfigurationRoot _configuration;

        public Startup(
            IHostingEnvironment environment,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Startup>();
            _configuration = ConfigurationSetup.Configure(environment);
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Add application configuration

            services.AddOptions();
            services.Configure<ApplicationOptions>(_configuration.GetSection("App"));

            #endregion Add application configuration

            #region Add IdentityServer

            var cert = new X509Certificate2(Path.Combine(
                _environment.ContentRootPath, "idsvr3test.pfx"), "idsrv3test");

            services.AddIdentityServer((options) =>
            {
                //options.RequireSsl = false;

                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.UserInteraction.LoginUrl = "/login";
                options.UserInteraction.LogoutUrl = "/logout";
                options.UserInteraction.ConsentUrl = "/consent";
                options.UserInteraction.ErrorUrl = "/error";
                options.Authentication.FederatedSignOutPaths.Add("/signout-oidc");
            })
                .AddTemporarySigningCredential()
                //AddExtensionGrantValidator<Extensions.ExtensionGrantValidator>()
                .AddProfileService<ProfileService>()
                .AddSecretParser<ClientAssertionSecretParser>()
                .AddSecretValidator<PrivateKeyJwtSecretValidator>()
                .AddSigningCredential(cert);

            #endregion Add IdentityServer

            // Add Data Layer
            if (String.IsNullOrWhiteSpace(_configuration["EntityFramework"]))
            {
                services.AddEntityFrameworkSqlServerStores(_configuration.GetSection("EntityFramework"));
            }
            else
            {
                throw new Exception("Store configuration not present");
            }

            #region Add Email Sender

            services.AddTransient<IEmailService, DebugEmailService>();
            /*services.AddTransient<IEmailService, DefaultEmailService>();
            services.Configure<DefaultEmailServiceOptions>(opt =>
            {
                opt.TemplateDirectoryPath = Path.Combine(_environment.ContentRootPath, "EmailTemplates");
            });

            if (String.IsNullOrWhiteSpace(_configuration["SendGrid"]))
            {
                services.Configure<SendGridOptions>(_configuration.GetSection("SendGrid"));
                services.AddTransient<IEmailSender, SendGridEmailSender>();
            }*/
            // else if o360
            // else if MailGun
            // else if SMTP
            // else default sender

            // services.AddTransient<IEmailFormatter, EmailFormatter>();

            #endregion Add Email Sender

            #region Add SMS Sender

            /*if (String.IsNullOrWhiteSpace(_configuration["Twillio"]))
            {
                services.Configure<TwillioOptions>(_configuration.GetSection("Twillio"));
                services.AddTransient<ISmsSender, TwillioSmsSender>();
            }*/

            #endregion Add SMS Sender

            services.AddTransient<ICrypto, DefaultCrypto>();
            services.AddTransient<UserAccountService>();
            services.AddTransient<ClientService>();

            // register event service
            services.AddAntiforgery();

            services
                .AddMvc()
                .AddRazorOptions(razor =>
                {
                    razor.ViewLocationExpanders.Add(new UI.CustomViewLocationExpander());
                });
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            /*Func<string, LogLevel, bool> filter = (scope, level) =>
                scope.StartsWith("IdentityServer") ||
                scope.StartsWith("IdentityModel") ||
                level == LogLevel.Error ||
                level == LogLevel.Critical;*/

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            /*if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }*/

            app.UseIdentityServer();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });

            #region Use third party authentication

            if (!String.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientId"]))
            {
                _logger.LogInformation("Registering Google authentication scheme");

                app.UseGoogleAuthentication(new GoogleOptions
                {
                    AuthenticationScheme = "Google",
                    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    ClientId = _configuration["Authentication:Google:ClientId"],
                    ClientSecret = _configuration["Authentication:Google:ClientSecret"]
                });
            }

            if (!String.IsNullOrWhiteSpace(_configuration["Authentication:Facebook:AppId"]))
            {
                _logger.LogInformation("Registering Facebook authentication scheme");

                app.UseFacebookAuthentication(new FacebookOptions()
                {
                    AuthenticationScheme = "Facebook",
                    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    AppId = _configuration["Authentication:Facebook:AppId"],
                    AppSecret = _configuration["Authentication:Facebook:AppSecret"]
                });
            }

            #endregion Use third party authentication

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.UseMiddleware<RequestIdMiddleware>();

            app.InitializeStores();
        }
    }
}