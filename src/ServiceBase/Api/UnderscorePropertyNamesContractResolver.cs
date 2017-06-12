using Newtonsoft.Json.Serialization;

namespace ServiceBase.Api
{
    public class UnderscorePropertyNamesContractResolver : DefaultContractResolver
    {
        public UnderscorePropertyNamesContractResolver() : base()
        {
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.Underscore();
        }
    }
}
