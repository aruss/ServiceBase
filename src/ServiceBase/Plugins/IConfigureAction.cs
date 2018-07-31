﻿// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using Microsoft.AspNetCore.Builder;

    public interface IConfigureAction
    {
        void Execute(IApplicationBuilder applicationBuilder);
    }
}
