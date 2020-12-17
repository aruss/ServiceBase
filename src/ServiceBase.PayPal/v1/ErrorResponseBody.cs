namespace ServiceBase.PayPal.V1
{
    using Newtonsoft.Json;

    public class ErrorResponseBody
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
