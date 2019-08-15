// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Options;
    using ServiceBase.Resources;

    public static class LocalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonFileBasedLocalization(
            this IServiceCollection services,
            Action<LocalizationOptions> localizationOptionsSetupAction,
            Action<RequestLocalizationOptions> requestLocalizationOptionsSetupAction)
        {
            services.Configure(localizationOptionsSetupAction);

            services.AddScoped<IResourceStore, InMemoryResourceStore>();
            services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();
            services.AddSingleton<IStringLocalizer, StringLocalizer>();
            services.AddScoped<LocalizationHelper>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IResourceStore resourceStore = serviceProvider
                .GetRequiredService<IResourceStore>();

            LocalizationOptions localizationOptions = serviceProvider
                .GetRequiredService<IOptions<LocalizationOptions>>().Value;

            resourceStore.LoadLocalizationFromDirectoryAsync(
                localizationOptions.ResourcesPath).Wait();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture =
                    new RequestCulture(localizationOptions.DefaultRequestCulture);

                IEnumerable<string> cultures =
                    resourceStore.GetAllLocalizationCulturesAsync().Result;

                options.SupportedCultures =
                options.SupportedUICultures =
                    cultures.Select(s => new CultureInfo(s)).ToList();

                requestLocalizationOptionsSetupAction(options);
            });

            return services;
        }
    }
}
