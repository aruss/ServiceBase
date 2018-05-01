namespace PluginA
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Shared;
    using ServiceBase.Mvc.Plugins;

    public class ConfigureAction : IConfigureAction
    {
        public void Execute(IApplicationBuilder applicationBuilder)
        {
            Console.WriteLine("PluginAPlugin execute ConfigureAction");

            // Initialize foo repository
            for (int i = 1; i <= 10; i++)
            {
                Guid id = Guid.NewGuid();

                FooMemoryMepository.Foos.TryAdd(id, new Foo
                {
                    Id = id,
                    Name = $"PluginA Foo #{i}"
                });
            }
        }
    }
}
