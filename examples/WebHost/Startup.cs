// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace WebHost
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Logging;
    using Serilog.Extensions.Logging;
    using Serilog;
    using ServiceBase.Plugins;
    using ServiceBase.Extensions;
    using Microsoft.AspNetCore.Mvc.Razor;
    using ServiceBase.Mvc.Theming;
    using Microsoft.AspNetCore.Http;
    using ServiceBase;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using System.Threading.Tasks;

 
    public class Startup
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationOptions _applicationOptions;
        private readonly string _pluginsPath;
        private readonly IWebHostEnvironment _environment;

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment environment
            )
        {
            this._configuration = configuration;
            this._environment = environment;      

            this._applicationOptions = this._configuration.GetSection("App")
                .Get<ApplicationOptions>() ?? new ApplicationOptions();

            #region Create aspnet core logger from serilog logger

            var factory = new SerilogLoggerFactory(Serilog.Log.Logger);
            this._logger = factory.CreateLogger("Startup");

            #endregion 

            #region Init plugin assembly loader

            string[] whiteList = this._configuration
                    .GetSection("Plugins")
                    .Get<string[]>();

            this._pluginsPath = this._applicationOptions.PluginsPath
                .GetFullPath(this._environment.ContentRootPath);

            PluginAssembyLoader.LoadAssemblies(
                this._pluginsPath,
                this._logger,
                whiteList);

            #endregion
        }

        // This method gets called by the runtime. Use this method to add
        // services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            this._logger.LogInformation("Configure services");

            services.AddSingleton<IDateTimeAccessor, DateTimeAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services
                .AddControllersWithViews(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.SuppressOutputFormatterBuffering = true;
                })
                .AddNewtonsoftJson()
                .AddRazorRuntimeCompilation(); 

            IThemeInfoProvider provider = new SimpleThemeInfoProvider(this._applicationOptions.ThemeName);
            services.AddSingleton(provider);
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Clear();
                options.ViewLocationExpanders.Add(
                     new ThemeViewLocationExpander(provider, this._applicationOptions.PluginsPath));
            });

            services.AddPluginsMvc(this._logger); 
            services.AddPlugins(this._logger);

            this._logger.LogInformation("Configure services completed");
        }

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            this._logger.LogInformation("Configure application");

            if (env.IsDevelopment())
            {
                this._logger
                    .LogInformation("Using development exception page");

                app.UseDeveloperExceptionPage();
            }
            else
            {
                this._logger
                    .LogInformation("Using exception handler /home/error");

                app.UseExceptionHandler("/Home/Error");
            }

            // Move it after the Static Files middleware to disable logging of
            // static file request 
            app.UseSerilogRequestLogging(options =>
            {
                // Attach additional properties to the request completion event
                options
                    .EnrichDiagnosticContext = LoggingUtils.EnrichFromRequest;
            });

            app.UsePluginsStaticFiles(this._pluginsPath);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UsePlugins(this._logger);
            app.UsePluginsMvc(this._logger);

            this._logger.LogInformation("Configure application completed");
        }
    }
}
