using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Services.InMemory;
using IdentityServer4.Stores;
using IdentityServer4.Stores.InMemory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Postgres;
using ServiceBase.IdentityServer.Services;
using ServiceBase.Notification.Email;
using ServiceBase.Notification.SMS;
using ServiceBase.Notification.Twilio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ServiceBase.IdentityServer.Public
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile(Path.Combine("Config", "config.json"), optional: false, reloadOnChange: true)
               .AddJsonFile(Path.Combine("Config", $"config.{env.EnvironmentName}.json"), optional: true, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();

            _configuration = builder.Build();
            _environment = env;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            #region Add application configuration 

            services.AddOptions();
            services.Configure<ApplicationOptions>(_configuration.GetSection("App"));
            services.AddSingleton<IConfiguration>(_configuration);

            #endregion

            #region Add IdentityServer 

            var cert = new X509Certificate2(Path.Combine(
                _environment.ContentRootPath, "idsvr3test.pfx"), "idsrv3test");

            var builder = services.AddIdentityServer((options) =>
            {
                //options.RequireSsl = false;                
                options.UserInteractionOptions.LoginUrl = "/login";
                options.UserInteractionOptions.LogoutUrl = "/logout";
                options.UserInteractionOptions.ConsentUrl = "/consent";
                options.UserInteractionOptions.ErrorUrl = "/error";
            })
            .AddInMemoryStores() // Development version
            .SetSigningCredential(cert);

            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<IClientStore, InMemoryClientStore>();
            services.AddTransient<ICorsPolicyService, InMemoryCorsPolicyService>();
            services.AddTransient<IScopeStore, InMemoryScopeStore>();

            services.AddSingleton(
                JsonConvert.DeserializeObject<IEnumerable<Client>>(
                    File.ReadAllText(Path.Combine(_environment.ContentRootPath, "Config", "clients.json")))
            );

            services.AddSingleton(
               JsonConvert.DeserializeObject<IEnumerable<Scope>>(
                   File.ReadAllText(Path.Combine(_environment.ContentRootPath, "Config", "scopes.json")))
            );

            #endregion

            #region Add Data Layer 

            services.AddPostgresStores(config =>
            {
                config.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            });

            #endregion

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

            #endregion

            #region Add SMS Sender 

            if (String.IsNullOrWhiteSpace(_configuration["Twillio"]))
            {
                services.Configure<TwillioOptions>(_configuration.GetSection("Twillio"));
                services.AddTransient<ISmsSender, TwillioSmsSender>();
            }

            #endregion 

            services.AddTransient<ICrypto, DefaultCrypto>();

            services
                .AddMvc()
                .AddRazorOptions(razor =>
                {
                    razor.ViewLocationExpanders.Add(new UI.CustomViewLocationExpander());
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            /*Func<string, LogLevel, bool> filter = (scope, level) =>
                scope.StartsWith("IdentityServer") ||
                scope.StartsWith("IdentityModel") ||
                level == LogLevel.Error ||
                level == LogLevel.Critical;*/

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

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
                app.UseFacebookAuthentication(new FacebookOptions()
                {
                    AuthenticationScheme = "Facebook",
                    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    AppId = _configuration["Authentication:Facebook:AppId"],
                    AppSecret = _configuration["Authentication:Facebook:AppSecret"]
                });
            }

            #endregion
            
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
