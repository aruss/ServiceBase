namespace ServiceBase.PayPal.V2
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// The tax information of the payer. Required only for Brazilian payer's.
    /// Both tax_id and tax_id_type are required.
    /// </summary>
    public class TaxInfo
    {
        /// <summary>
        /// The customer's tax ID. Supported for the PayPal payment method
        /// only. Typically, the tax ID is 11 characters long for individuals
        /// and 14 characters long for businesses.
        /// </summary>
        [JsonProperty("tax_id")]
        [MaxLength(14)]
        public string TaxId { get; set; }

        /// <summary>
        /// The customer's tax ID type. Supported for the PayPal payment
        /// method only. The possible values are: <see cref="TaxIdType"/>
        /// </summary>
        [JsonProperty("tax_id_type")]
        public string TaxIdType { get; set; }
    }
}

