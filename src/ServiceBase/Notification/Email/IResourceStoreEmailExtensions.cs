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
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using ServiceBase.Resources;

    public static class IResourceStoreEmailExtensions
    {
        private const string EmailGroup = "Email";

        /// <summary>
        /// Loads email templates into global dictionary from directory
        /// of json files.
        /// </summary>
        /// <param name="resourceStore">
        /// Instance of <see cref="IResourceStore"/>.
        /// </param>
        /// <param name="localePath">
        /// Path to directory containing localization files.
        /// File pattern should match following example "foo.de-DE.xml".
        /// </param>
        public static async Task LoadEmailTemplateFromDirectoryAsync(
            this IResourceStore resourceStore,
            string directoryPath,
            string source = null)
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(
                CultureTypes.AllCultures &
                ~CultureTypes.NeutralCultures);

            foreach (string filePath in
                Directory.GetFiles(directoryPath, "*.xml"))
            {
                string[] filePathChunks = Path.GetFileName(filePath)
                                                  .Split('.');

                if (filePathChunks.Length <= 2)
                {
                    // Not valid file name
                    continue;
                }

                string key = filePathChunks.First();

                string culture = filePathChunks
                    .ElementAtOrDefault(filePathChunks.Length - 2);

                CultureInfo cultureInfo = cultures
                    .FirstOrDefault(c => c.Name.Equals(
                        culture,
                        StringComparison.OrdinalIgnoreCase));

                if (culture == null)
                {
                    // Not valid culture code
                    continue;
                }

                using (StreamReader file = File.OpenText(filePath))
                {
                    var value = await file.ReadToEndAsync();

                    await resourceStore.WriteAsync(
                        cultureInfo.Name,
                        IResourceStoreEmailExtensions.EmailGroup,
                        key,
                        value,
                        source);
                }
            }
        }

        public static async Task<Resource> GetEmailTemplateAsync(
            this IResourceStore resourceStore,
            string culture,
            string key)
        {
            Resource resource = await resourceStore.GetAsync(
                culture,
                IResourceStoreEmailExtensions.EmailGroup,
                key);

            return resource;
        }
    }
}
