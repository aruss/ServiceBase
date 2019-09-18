// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using ServiceBase.Extensions;
    using ServiceBase.Resources;

    public static class LocalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonFileBasedLocalization(
            this IServiceCollection services,
            Action<LocalizationOptions> localizationOptionsSetupAction = null,
            Action<RequestLocalizationOptions> requestLocalizationOptionsSetupAction = null)
        {
            if (localizationOptionsSetupAction != null)
            {
                services.Configure(localizationOptionsSetupAction);
            }

            services.AddScoped<IResourceStore, InMemoryResourceStore>();
            services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();
            services.AddSingleton<IStringLocalizer, StringLocalizer>();
            services.AddScoped<LocalizationHelper>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IResourceStore resourceStore = serviceProvider
                .GetRequiredService<IResourceStore>();

            ILogger logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger("LocalizationServiceCollectionExtensions");

            LocalizationOptions localizationOptions = serviceProvider
                .GetRequiredService<IOptions<LocalizationOptions>>().Value;

            IHostingEnvironment hostingEnvironment = serviceProvider
                  .GetRequiredService<IHostingEnvironment>();

            string resourcePath = localizationOptions.ResourcesPath
                .GetFullPath(hostingEnvironment.ContentRootPath);

            logger.LogDebug("Loading resources from {0}", resourcePath); 

            resourceStore.LoadLocalizationFromDirectoryAsync(resourcePath, null, logger).Wait();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                logger.LogDebug("Setting default request culture to {0}",
                    localizationOptions.DefaultCulture);

                options.DefaultRequestCulture =
                    new RequestCulture(localizationOptions.DefaultCulture);

                IEnumerable<string> cultures =
                    resourceStore.GetAllLocalizationCulturesAsync().Result;

                logger.LogDebug("Setting supported cultures to {0}",
                    String.Join(", ", cultures)); 

                options.SupportedCultures =
                options.SupportedUICultures =
                    cultures.Select(s => new CultureInfo(s)).ToList();

                requestLocalizationOptionsSetupAction?.Invoke(options);
            });

            return services;
        }
    }
}
