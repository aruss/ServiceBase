namespace ServiceBase.Json
{
    using Newtonsoft.Json.Serialization;

    public class UnderscorePropertyNamesContractResolver :
        DefaultContractResolver
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
