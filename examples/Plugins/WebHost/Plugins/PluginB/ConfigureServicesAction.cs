namespace PluginB
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ServiceBase.Plugins;
    using Shared;
    using System;

    public class ConfigureServicesAction : IConfigureServicesAction
    {
        public void Execute(IServiceCollection services)
        {
            Console.WriteLine("PluginBPlugin execute ConfigureServicesAction");

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IConfiguration configuration = serviceProvider
                .GetService<IConfiguration>();

            string connectionString = configuration
                .GetConnectionString("PluginBConnection");

            services.AddDbContext<PluginBDbContext>(options =>
            { 
                options.UseSqlServer(connectionString);
            });

            services.AddTransient<IFooRepository, FooEFMepository>();
        }
    }
}
