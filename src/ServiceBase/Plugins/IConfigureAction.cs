namespace ServiceBase.Plugins
{
    using Microsoft.AspNetCore.Builder;

    public interface IConfigureAction
    {
        void Execute(IApplicationBuilder applicationBuilder);
    }
}
