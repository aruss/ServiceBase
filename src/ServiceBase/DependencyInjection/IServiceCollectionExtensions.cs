// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class IServiceCollectionExtensions
    {  
        public static IServiceCollection AddFactory<TService, TFactory>(
            this IServiceCollection collection,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            ServiceLifetime factoryLifetime = ServiceLifetime.Scoped)
            where TService : class
            where TFactory : class, IServiceFactory<TService>
        {
            collection.Add(
                new ServiceDescriptor(
                    typeof(IServiceFactory<TService>),
                    typeof(TFactory),
                    factoryLifetime
                )
            );
            
            return collection.Add<TService, TFactory>(
                p => p.GetRequiredService<IServiceFactory<TService>>(),
                serviceLifetime);
        }

        public static IServiceCollection Add<TService, TFactory>(
            this IServiceCollection collection,
            Func<IServiceProvider, IServiceFactory<TService>> factoryProvider,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TService : class
            where TFactory : class, IServiceFactory<TService>
        {
            object factoryFunc(IServiceProvider provider)
            {
                IServiceFactory<TService> factory = factoryProvider(provider);
                return factory.Build();
            }

            collection.Add(
                new ServiceDescriptor(typeof(TService), factoryFunc, lifetime));

            return collection;
        }
    }
}
