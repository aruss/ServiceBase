// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace PluginB
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Plugins;

    public class ConfigureAction : IConfigureAction
    {
        public void Execute(IApplicationBuilder applicationBuilder, ILogger logger)
        {
            logger.LogInformation("PluginB execute ConfigureAction");
        }
    }
}
