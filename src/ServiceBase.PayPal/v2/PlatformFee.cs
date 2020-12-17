namespace ServiceBase.PayPal.V2
{
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-platform_fee
    /// </summary>
    public class PlatformFee
    {
        /// <summary>
        /// The fee for this transaction.
        /// </summary>
        public Money Amount { get; set; }

        /// <summary>
        /// The recipient of the fee for this transaction. If you omit this
        /// value, the default is the API caller.
        /// </summary>
        [JsonProperty("payee")]
        public Payee Payee { get; set; }
    }
}
