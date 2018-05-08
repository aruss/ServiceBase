// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Resources;

    /// <summary>
    /// Represents a service that provides localized strings.
    /// </summary>
    public class StringLocalizer : IStringLocalizer
    {
        private readonly IResourceStore _resourceStore;
        private readonly ILogger<StringLocalizer> _logger;

        public StringLocalizer(
            IResourceStore resourceStore,
            ILogger<StringLocalizer> logger)
        {
            this._resourceStore = resourceStore;
            this._logger = logger;
        }

        private string GetTranslation(string key)
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;

            Resource resource = this._resourceStore
                .GetLocalizationAsync(culture.Name, key)
                .Result;

            if (resource == null ||
                string.IsNullOrWhiteSpace(resource.Value))
            {
                this._logger.LogWarning($"No localization found for \"{key}\"");
                return key;
            }

            return resource.Value;
        }

        public LocalizedString this[string name]
        {
            get
            {
                return new LocalizedString(name, this.GetTranslation(name));
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                string translation = this.GetTranslation(name);

                return new LocalizedString(name,
                    string.Format(translation, arguments));
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(
            bool includeParentCultures)
        {
            throw new System.NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
