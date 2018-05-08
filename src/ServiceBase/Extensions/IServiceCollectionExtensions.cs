﻿namespace ServiceBase.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection TryRemove<TService>(
           this IServiceCollection services)
        {
            ServiceDescriptor descriptorToRemove = services
               .FirstOrDefault(d => d.ServiceType == typeof(TService));

            if (descriptorToRemove != null)
            {
                services.Remove(descriptorToRemove);
            }

            return services;
        }

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
            services.TryRemove<TService>();

            ServiceDescriptor descriptorToAdd = new ServiceDescriptor(
                typeof(TService),
                typeof(TImplementation),
                lifetime
            );

            services.Add(descriptorToAdd);

            return services;
        }

        /// <summary>
        /// Replaces registerd <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <param name="implementationFactory"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IServiceCollection Replace<TService, TImplementation>(
             this IServiceCollection services,
             Func<IServiceProvider, TImplementation> implementationFactory,
             ServiceLifetime lifetime)
             where TService : class
             where TImplementation : class, TService
        {
            services.TryRemove<TService>();

            ServiceDescriptor descriptorToAdd = new ServiceDescriptor(
                typeof(TService),
                implementationFactory,
                lifetime
            );

            services.Add(descriptorToAdd);

            return services;
        }

        public static IServiceCollection Replace<TService, TImplementation>(
             this IServiceCollection services,
             TImplementation implementation)
             where TService : class
        {
            services.TryRemove<TService>();

            ServiceDescriptor descriptorToAdd = new ServiceDescriptor(
                typeof(TService),
                implementation);

            services.Add(descriptorToAdd);

            return services;
        }

        public static IServiceCollection TryAdd<TService, TImplementation>(
           this IServiceCollection services,
           ServiceLifetime lifetime)
           where TService : class
           where TImplementation : class, TService
        {
            ServiceDescriptor descriptorToRemove = services
                .FirstOrDefault(d => d.ServiceType == typeof(TService));

            if (descriptorToRemove == null)
            {
                ServiceDescriptor descriptorToAdd = new ServiceDescriptor(
                    typeof(TService),
                    typeof(TImplementation),
                    lifetime
                );

                services.Add(descriptorToAdd);
            }

            return services;
        }

        public static IServiceCollection TryAddSingleton
            <TService, TImplementation>(
          this IServiceCollection services)
          where TService : class
          where TImplementation : class, TService
        {
            return services.TryAdd<TService, TImplementation>(
                ServiceLifetime.Singleton);
        }
    }
}