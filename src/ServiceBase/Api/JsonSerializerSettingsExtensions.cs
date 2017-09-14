namespace ServiceBase.Api
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public static class JsonSerializerSettingsExtensions
    {
        public static JsonSerializerSettings ConfigureCommon(
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
    }
}
