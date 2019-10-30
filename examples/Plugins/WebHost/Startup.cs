#define DYNAMIC

namespace WebHost
{
    using System;
    using System.IO;
    using System.Net.Http;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ServiceBase;
    using ServiceBase.Logging;
    using ServiceBase.Plugins;

    public class Startup : IStartup
    {
        private readonly ILogger<Startup> _logger;
        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _pluginsPath;

        /// <summary>
        ///
        /// </summary>
        /// <param name="configuration">Instance of <see cref="configuration"/>
        /// </param>
        /// <param name="environment">Instance of
        /// <see cref="IHostingEnvironment"/></param>
        /// <param name="logger">Instance of <see cref="ILogger{Startup}"/>
        /// </param>
        public Startup(
            IConfiguration configuration,
            IHostingEnvironment environment,
            ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor,
            Func<HttpMessageHandler> messageHandlerFactory = null)
        {
            this._logger = loggerFactory.CreateLogger<Startup>();
            this._environment = environment;
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;

#if DYNAMIC
            this._pluginsPath = Path.GetFullPath(
                Path.Combine(environment.ContentRootPath, "plugins"));

            string[] whiteList = this._configuration
                .GetSection("Plugins")
                .Get<string[]>();

            PluginAssembyLoader.LoadAssemblies(
                this._pluginsPath,
                this._logger,
                whiteList);
#else
            // Statically add plugin assemblies for debugging 
            // You can add and remove active plugins here

            this._pluginsPath = Path.GetFullPath(
                Path.Combine(environment.ContentRootPath, "Plugins"));

            //Console.WriteLine("Loading plugins statically.");
            //Console.WriteLine(typeof(PluginA.PluginAInfo));
            //Console.WriteLine(typeof(PluginB.PluginBPlugin));
#endif
        }

        /// <summary>
        /// Configurates the services.
        /// </summary>
        /// <param name="services">
        /// Instance of <see cref="IServiceCollection"/>.
        /// </param>
        /// <returns>
        /// Instance of <see cref="IServiceProvider"/>.
        /// </returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            this._logger.LogInformation("Configure services.");

            services.AddSingleton(this._configuration);
            services.AddHttpClient();

            services.AddAntiforgery((options) =>
            {
                options.Cookie.Name = "srf";
            });

            //services
            //   .AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //services.AddDistributedMemoryCache();


            services.AddPlugins();
            services.AddPluginsMvc(this._logger);

            this._logger.LogInformation("Services configured.");

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Configures the pipeline.
        /// </summary>
        /// <param name="app">
        /// Instance of <see cref="IApplicationBuilder"/>.
        /// </param>
        public virtual void Configure(IApplicationBuilder app)
        {
            this._logger.LogInformation("Configure application.");

            IHostingEnvironment env = app.ApplicationServices
                .GetRequiredService<IHostingEnvironment>();

            app.UseMiddleware<RequestIdMiddleware>();
            app.UseSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UsePlugins();
            app.UsePluginsMvc();
            app.UsePluginsStaticFiles(this._pluginsPath);

            this._logger.LogInformation("Configure application.");
        }
    }
}
