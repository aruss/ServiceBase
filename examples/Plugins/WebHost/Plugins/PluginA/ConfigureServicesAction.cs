namespace PluginA
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using ServiceBase.Mvc.Plugins;
    using Shared;

    public class ConfigureServicesAction : IConfigureServicesAction
    {
        public void Execute(IServiceCollection services)
        {
            Console.WriteLine("PluginAPlugin execute ConfigureServicesAction");

            services.AddTransient<IFooRepository, FooMemoryMepository>(); 
        }
    }
}
