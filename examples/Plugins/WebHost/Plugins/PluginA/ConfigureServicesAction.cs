namespace PluginA
{
    using Microsoft.Extensions.DependencyInjection;
    using ServiceBase.Plugins;
    using Shared;
    using System;

    public class ConfigureServicesAction : IConfigureServicesAction
    {
        public void Execute(IServiceCollection services)
        {
            Console.WriteLine("PluginAPlugin execute ConfigureServicesAction");

            services.AddTransient<IFooRepository, FooMemoryMepository>(); 
        }
    }
}
