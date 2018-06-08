namespace ThemeA
{
    using Microsoft.AspNetCore.Builder;
    using ServiceBase.Plugins;
    using System;

    public class ConfigureAction : IConfigureAction
    {
        public void Execute(IApplicationBuilder applicationBuilder)
        {
            Console.WriteLine("ThemePlugin execute ConfigureAction");
        }
    }
}
