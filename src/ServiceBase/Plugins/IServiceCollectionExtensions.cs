namespace ServiceBase.Plugins
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static partial class IServiceCollectionExtensions
    {
        public static void AddPlugins(
            this IServiceCollection serviceCollection)
        {
            IServiceProvider serviceProvider =
                serviceCollection.BuildServiceProvider();

            ILogger logger = serviceProvider
                 .GetService<ILoggerFactory>()
                 .CreateLogger(typeof(IServiceCollectionExtensions));

            IEnumerable<IConfigureServicesAction> actions =
                PluginAssembyLoader.GetServices<IConfigureServicesAction>();

            foreach (IConfigureServicesAction action in actions)
            {
                logger.LogInformation(
                    "Executing ConfigureServices action '{0}'",
                    action.GetType().FullName);

                action.Execute(serviceCollection);
            }
        }
    }
}
