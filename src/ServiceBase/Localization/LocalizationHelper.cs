// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using ServiceBase.Resources;

    public class LocalizationHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RequestLocalizationOptions _requestLocalizationOptions;
        private readonly LocalizationOptions _localizationOptions;
        private readonly IResourceStore _resourceStore;
        private readonly IDictionary<string, string> _mappedNativeNames;

        public LocalizationHelper(
            IHttpContextAccessor httpContextAccessor,
            IResourceStore resourceStore,
            IOptions<RequestLocalizationOptions> requestLocalizationOptions,
            IOptions<LocalizationOptions> localizationOptions)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._resourceStore = resourceStore;
            this._requestLocalizationOptions = requestLocalizationOptions.Value;
            this._localizationOptions = localizationOptions.Value;
            this._mappedNativeNames = this.ReadCultureMap();
        }

        /// <summary>
        /// Replaces the culture segment in the url, url must be of the
        /// following format /{cutlure}/path/to/what/ever
        /// </summary>
        /// <param name="currentUrl">Url with culture to replace.</param>
        /// <param name="culture">A predefined System.Globalization.CultureInfo
        /// name, <see cref="System.Globalization.CultureInfo.Name"/> of an existing
        /// <see cref="System.Globalization.CultureInfo"/>, or Windows-only culture name.
        /// name is not case-sensitive.</param>
        /// <returns>A new replaced URL</returns>
        public static string ReplaceCulture(string currentUrl, string culture)
        {
            string[] segments1 = currentUrl.Split('/')
                .Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();

            string[] segments2 = segments1[0].Split('?');

            segments2[0] = culture;
            segments1[0] = string.Join('?', segments2);

            return '/' + string.Join('/', segments1);
        }

        /// <summary>
        /// Reads the CultureMap.json file with CultureInfo.NativeName overrides.
        /// </summary>
        /// <returns>
        /// A dictionary with CultureInfo name as key and native name override as value.
        /// </returns>
        private IDictionary<string, string> ReadCultureMap()
        {
            using (StreamReader r = new StreamReader(Path.Combine(
                this._localizationOptions.ResourcesPath, "CultureMap.json")))
            {
                string json = r.ReadToEnd();
                IDictionary<string, string> map = JsonConvert
                    .DeserializeObject<Dictionary<string, string>>(json);

                // map to lower case keys 
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                return new Dictionary<string, string>(map, comparer);
            }
        }

        /// <summary>
        /// Tries to find a mapped culture native name, if no mapped culture
        /// native name is provided then default native name will be returned 
        /// </summary>
        /// <param name="culture">A predefined System.Globalization.CultureInfo
        /// name, <see cref="System.Globalization.CultureInfo.Name"/> of an existing
        /// <see cref="System.Globalization.CultureInfo"/>, or Windows-only culture name.
        /// name is not case-sensitive.</param>
        /// <returns>A mapped native culture name</returns>
        public string GetMappedNativeName(string culture)
        {
            if (this._mappedNativeNames.ContainsKey(culture))
            {
                return this._mappedNativeNames[culture];
            }

            var cultureInfo = new CultureInfo(culture);
            if (cultureInfo != null)
            {
                return cultureInfo.NativeName;
            }

            return culture;
        }

        /// <summary>
        /// Gets a list of supported cultures
        /// </summary>
        /// <param name="sortByAcceptLanguageHeader">
        /// If true the list will be presorted with respect of Accept-Language header.
        /// NOT YET IMPLEMENTED
        /// </param>
        /// <returns>
        /// A list of <see cref="CultureInfo"/> instances 
        /// </returns>
        public Task<IList<CultureInfo>> GetSupportedUICulturesAsync(
            bool sortByAcceptLanguageHeader = true)
        {
            HttpContext context = this._httpContextAccessor.HttpContext;

            // sortByAcceptLanguageHeader not supported yet 
            // en-US,en;q=0.9,de;q=0.8,ru;q=0.7
            // var acceptLanguageHeader = context.Request.Headers["Accept-Language"];

            var requestCulture = context.Features.Get<IRequestCultureFeature>();
            var cultureItems = this._requestLocalizationOptions.SupportedUICultures;

            return Task.FromResult(cultureItems);
        }

        /// <summary>
        /// Returns a dictionary with resources for specific culture 
        /// </summary>
        /// <param name="culture">A predefined System.Globalization.CultureInfo
        /// name, <see cref="System.Globalization.CultureInfo.Name"/> of an existing
        /// <see cref="System.Globalization.CultureInfo"/>, or Windows-only culture name.
        /// name is not case-sensitive.</param>
        /// <returns>
        /// A <see cref="IDictionary{string, string}"/> with all resources for specific culture.
        /// </returns>
        public async Task<IDictionary<string, string>> GetAllResourcesAsync(
            string culture)
        {
            return (await this._resourceStore
                .GetAllLocalizationAsync(culture))
                .ToDictionary(s => s.Key, s => s.Value);
        }
    }
}
