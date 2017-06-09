using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ServiceBase.Extensions
{
    public static class JsonSerializerSettingsExtensions
    {
        public static JsonSerializerSettings ConfigureCommon(this JsonSerializerSettings settings)
        {
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;

            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

            return settings; 
        }
    }
}
