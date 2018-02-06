namespace ServiceBase.ExtensionHost
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    public interface IExtension
    {
        string Name { get; set; }
    }

    public interface IConfigureAction
    {
        void Execute(
            IApplicationBuilder app, 
            IServiceProvider serviceProvider);
    }

    public interface IConfigureServicesAction
    {
        void Execute(
            IServiceCollection services, 
            IServiceProvider serviceProvider);
    }

    public interface IUseMvcAction
    {
        void Execute(
            IRouteBuilder routeBuilder, 
            IServiceProvider serviceProvider);
    }

    public interface IAddMvcAction
    {
        void Execute(
            IMvcBuilder mvcBuilder,
            IServiceProvider serviceProvider);
    }
}
