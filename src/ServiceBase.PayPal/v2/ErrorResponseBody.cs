namespace ServiceBase.PayPal.V2
{
    using Newtonsoft.Json;

    public class ErrorResponseBody
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("debug_id")]
        public string DebugId { get; set; }

        [JsonProperty("details")]
        public ErrorDetails[] Details { get; set; }

        [JsonProperty("links")]
        public LinkDescription[] Links { get; set; }
    }
}
