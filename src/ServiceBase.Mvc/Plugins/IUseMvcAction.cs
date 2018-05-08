namespace ServiceBase.Plugins
{
    using Microsoft.AspNetCore.Routing;

    public interface IUseMvcAction
    {
        void Execute(IRouteBuilder routeBuilder);
    }
}
