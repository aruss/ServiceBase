// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Json
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public static class JsonSerializerSettingsExtensions
    {
        /// <summary>
        /// Configures <see cref="JsonSerializerSettings"/> with common
        /// restfull API settings.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static JsonSerializerSettings SetupDefaults(
            this JsonSerializerSettings settings)
        {
            settings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();

            //settings.ContractResolver =
            //  new UnderscorePropertyNamesContractResolver();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;

            settings.Converters.Add(
                new StringEnumConverter { CamelCaseText = true }
            );

            return settings;
        }

        public static JsonSerializerSettings CreateWithDefaults()
        {
            return new JsonSerializerSettings().SetupDefaults();
        }
    }
}