// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public static class IResourceStoreLocalizationExtensions
    {
        private const string LocalizationGroup = "Localization";

        /// <summary>
        /// Loads localization data into global dictioanry from directory
        /// of json files.
        /// </summary>
        /// <param name="resourceStore">
        /// Instance of <see cref="IResourceStore"/>.
        /// </param>
        /// <param name="localePath">
        /// Path to directory containing localization files.
        /// File pattern should match following example "foo.de-DE.json".
        /// </param>
        public static async Task LoadLocalizationFromDirectoryAsync(
            this IResourceStore resourceStore,
            string directoryPath,
            string source = null,
            ILogger logger = null)
        {
            JsonSerializer serializer = new JsonSerializer();

            IEnumerable<CultureInfo> cultures = CultureInfo.GetCultures(
                CultureTypes.AllCultures &
                ~CultureTypes.NeutralCultures);

            logger?.LogDebug("Supported cultures {0}: {1}",
                cultures.Count(), String.Join(", ", cultures));

            foreach (var filePath in
                Directory.GetFiles(directoryPath, "*.json"))
            {
                logger?.LogDebug("Try loading resource file {0}", filePath);

                string[] filePathChunks = filePath.Split('.');

                if (filePathChunks.Length <= 2)
                {
                    logger?.LogDebug("Not valid resource file name {0}", filePath);
                    continue;
                }

                string cultureCode = filePathChunks
                    .ElementAtOrDefault(filePathChunks.Length - 2);

                CultureInfo culture = cultures
                    .FirstOrDefault(c => c.Name.Equals(
                        cultureCode,
                        StringComparison.OrdinalIgnoreCase));

                if (culture == null)
                {
                    logger?.LogError(
                        "Not valid culture {0} in file name {1}", cultureCode, filePath);

                    continue;
                }

                logger?.LogDebug("Reading resource file {1}", filePath);

                using (StreamReader file = File.OpenText(filePath))
                {
                    Dictionary<string, string> keyValuePairs =
                        (Dictionary<string, string>)serializer
                        .Deserialize(file, typeof(Dictionary<string, string>));

                    await resourceStore.WriteAsync(
                        culture.Name,
                        IResourceStoreLocalizationExtensions.LocalizationGroup,
                        keyValuePairs,
                        source);
                }
            }
        }

        public static async Task<Resource> GetLocalizationAsync(
            this IResourceStore resourceStore,
            string culture,
            string key)
        {
            Resource resource = await resourceStore.GetAsync(
                culture,
                IResourceStoreLocalizationExtensions.LocalizationGroup,
                key);

            return resource;
        }

        public static async Task<IEnumerable<Resource>> GetAllLocalizationAsync(
            this IResourceStore resourceStore,
            string culture)
        {
            IEnumerable<Resource> resources = await resourceStore.GetAllAsync(
                culture,
                IResourceStoreLocalizationExtensions.LocalizationGroup);

            return resources;
        }

        public static async Task<IEnumerable<string>>
            GetAllLocalizationCulturesAsync(this IResourceStore resourceStore)
        {
            IEnumerable<string> result = await resourceStore
                .GetAllCulturesAsync(
                    IResourceStoreLocalizationExtensions.LocalizationGroup);

            return result;
        }
    }
}
