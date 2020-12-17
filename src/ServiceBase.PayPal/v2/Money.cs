namespace ServiceBase.PayPal.V2
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-money
    /// </summary>
    public class Money
    {
        /// <summary>
        /// The three-character ISO-4217 currency code that identifies the currency.
        /// </summary>
        [JsonProperty("currency_code")]
        [MaxLength(3)]
        [MinLength(3)]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// The value, which might be:
        /// An integer for currencies like JPY that are not typically fractional.
        /// A decimal fraction for currencies like TND that are subdivided into thousandths.
        /// For the required number of decimal places for a currency code, see Currency Codes.
        /// </summary>
        [JsonProperty("value")]
        [MaxLength(32)]
        public string Value { get; set; }
    }
}
