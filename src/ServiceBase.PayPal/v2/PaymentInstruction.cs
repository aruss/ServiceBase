namespace ServiceBase.PayPal.V2
{
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-payment_instruction 
    /// </summary>
    public class PaymentInstruction
    {
        /// <summary>
        /// An array of various fees, commissions, tips, or donations.
        /// </summary>
        [JsonProperty("platform_fees ")]
        public PlatformFee[] PlatformFees { get; set; }

        /// <summary>
        /// The funds that are held on behalf of the merchant.
        /// The possible values are: <see cref="DisbursementModes"/>
        /// Default: INSTANT.
        /// </summary>
        [JsonProperty("disbursement_mode ")]
        public string DisbursementMode { get; set; }
    }
}
