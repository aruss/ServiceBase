namespace ServiceBase.PayPal.V2
{
    using Newtonsoft.Json;

    public class ErrorDetails
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("issue")]
        public string Issue { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
