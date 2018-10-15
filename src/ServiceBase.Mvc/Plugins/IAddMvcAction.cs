namespace ServiceBase.Plugins
{
    using Microsoft.Extensions.DependencyInjection;

    public interface IAddMvcAction
    {
        void Execute(IMvcBuilder mvcBuilder);
    }
}
