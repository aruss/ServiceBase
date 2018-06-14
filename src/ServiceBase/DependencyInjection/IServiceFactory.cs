// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.DependencyInjection
{
    public interface IServiceFactory<TService> where TService : class
    {
        TService Build();
    }
}
