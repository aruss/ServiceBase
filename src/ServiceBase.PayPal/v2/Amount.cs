namespace ServiceBase.PayPal.V2
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-amount_with_breakdown
    /// </summary>
    public class AmountWithBreakdown
    {
        /// <summary>
        /// The value, which might be:
        ///  - An integer for currencies like JPY that are not typically fractional.
        ///  - A decimal fraction for currencies like TND that are subdivided into thousandths.
        /// </summary>
        [JsonProperty("value")]
        [MaxLength(32)]
        public string Value { get; set; }

        /// <summary>
        /// The three-character ISO-4217 currency code that identifies the currency.
        /// https://developer.paypal.com/docs/api/reference/currency-codes/
        /// </summary>
        [JsonProperty("currency_code")]
        [MinLength(3)]
        [MaxLength(3)]
        public string CurrencyCode { get; set; }
    }
}
