namespace ServiceBase.PayPal.V2
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-payer
    /// </summary>
    public class Payer
    {
        /// <summary>
        /// The name of the payer. Supports only the given_name and surname properties.
        /// </summary>
        [JsonProperty("name")]
        public PayerName Name { get; set; }

        /// <summary>
        /// The email address of the payer.
        /// </summary>
        [JsonProperty("email_address")]
        [MaxLength(254)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The PayPal assigned ID for the payer.
        /// Pattern: ^[2-9A-HJ-NP-Z]{13}$.
        /// </summary>
        [JsonProperty("payer_id")]
        [MaxLength(13)]
        [MinLength(13)]
        public string PayerId { get; set; }

        /// <summary>
        /// The phone number of the customer. Available only when you enable
        /// the Contact Telephone Number option in the Profile & Settings for
        /// the merchant's PayPal account. The phone.phone_number supports
        /// only national_number.
        /// </summary>
        [JsonProperty("phone")]
        public PhoneWithType Phone { get; set; }

        /// <summary>
        /// The birth date of the payer in YYYY-MM-DD format.
        /// Pattern: ^[0-9]{4}-(0[1-9]|1[0-2])-(0[1-9]|[1-2] [0-9]|3[0-1])$.
        /// </summary>
        [JsonProperty("birth_date")]
        [MaxLength(10)]
        [MinLength(10)]
        public string BirthDate { get; set; }

        /// <summary>
        /// The tax information of the payer. Required only for Brazilian
        /// payer's. Both tax_id and tax_id_type are required.
        /// </summary>
        [JsonProperty("tax_info")]
        public TaxInfo TaxInfo { get; set; }

        /// <summary>
        /// The address of the payer.
        /// Supports only the address_line_1, address_line_2, admin_area_1,
        /// admin_area_2, postal_code, and country_code properties.
        /// Also referred to as the billing address of the customer.
        /// </summary>
        [JsonProperty("address")]
        public AddressPortable Address { get; set; }
    }
}

