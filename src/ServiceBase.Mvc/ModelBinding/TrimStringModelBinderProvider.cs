// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc.ModelBinding
{
    // .AddMvc(options =>
    // {
    //     options.ModelBinderProviders
    //         .Insert(0, new TrimStringModelBinderProvider()); 
    // })

    using System;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class TrimStringModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder binder = new TrimStringModelBinder();

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(String) ? binder : null;
        }
    }
}
