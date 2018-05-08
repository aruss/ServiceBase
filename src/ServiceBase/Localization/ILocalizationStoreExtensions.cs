namespace ServiceBase.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public static partial class ILocalizationStoreExtensions
    {
        /// <summary>
        /// Loads localization data into global dictioanry from directory
        /// of json files.
        /// </summary>
        /// <param name="localizationStore">
        /// Instance of <see cref="ILocalizationStore"/>.
        /// </param>
        /// <param name="localePath">
        /// Path to directory containing localization files.
        /// File pattern should match following example "foo.de-DE.json".
        /// </param>
        public static async Task LoadLocalizationFromDirectoryAsync(
            this ILocalizationStore localizationStore,
            string localePath)
        {
            JsonSerializer serializer = new JsonSerializer();

            CultureInfo[] cultures = CultureInfo.GetCultures(
                CultureTypes.AllCultures &
                ~CultureTypes.NeutralCultures);

            foreach (var filePath in Directory.GetFiles(localePath, "*.json"))
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

                    await localizationStore
                        .WriteAsync(culture.Name, keyValuePairs);
                }
            }
        }
    }
}
