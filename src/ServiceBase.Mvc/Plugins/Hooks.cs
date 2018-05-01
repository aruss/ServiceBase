namespace ServiceBase.Mvc.Plugins
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    public interface IPlugin
    {
        //string Name { get; set; }
        
    }

    public interface IConfigureAction
    {
        void Execute(IApplicationBuilder applicationBuilder);
    }

    public interface IConfigureServicesAction
    {
        void Execute(IServiceCollection serviceCollection);
    }

    public interface IUseMvcAction
    {
        void Execute(IRouteBuilder routeBuilder);
    }

    public interface IAddMvcAction
    {
        void Execute(IMvcBuilder mvcBuilder);
    }
}
