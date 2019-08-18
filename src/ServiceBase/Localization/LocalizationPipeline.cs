// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Options;

    public class LocalizationPipeline
    {
        public void Configure(
            IApplicationBuilder app,
            IOptions<RequestLocalizationOptions> options)
        {
            app.UseRequestLocalization(options.Value);
            app.UseMiddleware<UnsupportedCulturesMiddleware>();
        }
    }
}

