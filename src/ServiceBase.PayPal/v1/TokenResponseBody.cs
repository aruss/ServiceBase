namespace ServiceBase.PayPal.V1
{
    using Newtonsoft.Json;

    public class TokenResponseBody
    {
        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}
