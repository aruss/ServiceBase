namespace WebHost
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ServiceBase.Mvc.Theming;
    using ServiceBase.Plugins;

    public class Startup : IStartup
    {
        public IConfiguration Configuration { get; }

        private readonly string _pluginsPath; 

        public Startup(
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            Configuration = configuration;



#if DYNAMIC
            // Load plugins dynamically at tuntime 
            Console.WriteLine("Loading plugins dynamically");

            this._pluginsPath = Path.GetFullPath(
                Path.Combine(environment.ContentRootPath, "plugins"));

            PluginAssembyLoader.LoadAssemblies(this._pluginsPath);
#else
            // Statically add plugin assemblies for debugging 
            // You can add and remove active plugins here

            this._pluginsPath = Path.GetFullPath(
                Path.Combine(environment.ContentRootPath, "Plugins"));

            Console.WriteLine("Loading plugins statically.");
            Console.WriteLine(typeof(PluginA.PluginAInfo));
            //Console.WriteLine(typeof(PluginB.PluginBPlugin));
#endif
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddPlugins();
            services.AddPluginsMvc();

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UsePlugins();
            app.UsePluginsMvc(); 
        }
    }
}
