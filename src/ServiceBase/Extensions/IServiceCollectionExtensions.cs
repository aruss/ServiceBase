
namespace ServiceBase.Extensions
{
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Replaces registerd <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IServiceCollection Replace<TService, TImplementation>(
             this IServiceCollection services,
             ServiceLifetime lifetime)
             where TService : class
             where TImplementation : class, TService
        {
            ServiceDescriptor descriptorToRemove = services
                .FirstOrDefault(d => d.ServiceType == typeof(TService));

            services.Remove(descriptorToRemove);

            ServiceDescriptor descriptorToAdd = new ServiceDescriptor(
                typeof(TService),
                typeof(TImplementation),
                lifetime
            );

            services.Add(descriptorToAdd);
            
            return services;
        }
    }
}
