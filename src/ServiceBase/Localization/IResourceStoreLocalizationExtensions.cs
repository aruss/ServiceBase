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
            string source = null)
        {
            JsonSerializer serializer = new JsonSerializer();

            CultureInfo[] cultures = CultureInfo.GetCultures(
                CultureTypes.AllCultures &
                ~CultureTypes.NeutralCultures);

            foreach (var filePath in
                Directory.GetFiles(directoryPath, "*.json"))
            {
                string[] filePathChunks = filePath.Split('.');

                if (filePathChunks.Length <= 2)
                {
                    // Not valid file name
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
                    // Not valid culture code
                    continue;
                }

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
