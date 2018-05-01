﻿namespace ThemeA
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using ServiceBase.Mvc.Plugins;

    public class ConfigureServicesAction : IConfigureServicesAction
    {
        public void Execute(IServiceCollection services)
        {
            Console.WriteLine("ThemePlugin execute ConfigureServicesAction");
        }
    }
}
