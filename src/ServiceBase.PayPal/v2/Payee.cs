namespace ServiceBase.PayPal.V2
{
    using Newtonsoft.Json;

    public class Payee
    {
        /// <summary>
        /// The merchant who receives payment for this transaction.
        /// </summary>
        [JsonProperty("email_address ")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The merchant who receives payment for this transaction.
        /// </summary>
        [JsonProperty("merchant_id ")]
        public string MerchantId { get; set; }
    }
}
