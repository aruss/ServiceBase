namespace ServiceBase.Plugins
{
    using Microsoft.Extensions.DependencyInjection;

    public interface IConfigureServicesAction
    {
        void Execute(IServiceCollection serviceCollection);
    }
}
