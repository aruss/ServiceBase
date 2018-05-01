namespace PluginB
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using ServiceBase.Mvc.Plugins;

    public class ConfigureAction : IConfigureAction
    {
        public void Execute(IApplicationBuilder applicationBuilder)
        {
            Console.WriteLine("PluginBPlugin execute ConfigureAction");

            PluginBDbContext pluginBDbContext = applicationBuilder
                .ApplicationServices
                .GetService<PluginBDbContext>();

            PluginBDbContextInitializer.Initialize(pluginBDbContext);
        }
    }
}
