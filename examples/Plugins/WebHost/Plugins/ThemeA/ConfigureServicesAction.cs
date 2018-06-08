namespace ThemeA
{
    using Microsoft.Extensions.DependencyInjection;
    using ServiceBase.Plugins;
    using System;

    public class ConfigureServicesAction : IConfigureServicesAction
    {
        public void Execute(IServiceCollection services)
        {
            Console.WriteLine("ThemePlugin execute ConfigureServicesAction");
        }
    }
}
