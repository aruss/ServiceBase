using Newtonsoft.Json;
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

            return settings; 
        }
    }
}
