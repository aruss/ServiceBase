﻿// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using Microsoft.Extensions.DependencyInjection;

    public interface IAddMvcAction
    {
        void Execute(IMvcBuilder mvcBuilder);
    }
}
