namespace ServiceBase.PayPal.V2
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// The address of the payer.
    /// Supports only the address_line_1, address_line_2, admin_area_1,
    /// admin_area_2, postal_code, and country_code properties.
    /// Also referred to as the billing address of the customer.
    /// </summary>
    public class AddressPortable
    {
        /// <summary>
        /// The first line of the address.For example, number or street.
        /// For example, 173 Drury Lane. Required for data entry and
        /// compliance and risk checks.Must contain the full address.
        /// </summary>
        [JsonProperty("address_line_1")]
        [MaxLength(300)]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// The second line of the address.For example, suite or apartment number.
        /// </summary>
        [JsonProperty("address_line_2")]
        [MaxLength(300)]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// The highest level sub-division in a country, which is usually a
        /// province, state, or ISO-3166-2 subdivision.Format for postal
        /// delivery. For example, CA and not California. Value, by country
        /// </summary>
        [JsonProperty("admin_area_1")]
        [MaxLength(300)]
        public string AdminArea1 { get; set; }

        /// <summary>
        /// A city, town, or village. Smaller than admin_area_level_1.
        /// </summary>
        [JsonProperty("admin_area_2")]
        [MaxLength(120)]
        public string AdminArea2 { get; set; }

        /// <summary>
        /// The postal code, which is the zip code or equivalent.
        /// Typically required for countries with a postal code or an
        /// equivalent. See postal code.
        /// </summary>
        [JsonProperty("postal_code")]
        [MaxLength(60)]
        public string PostalCode { get; set; }

        /// <summary>
        /// The two-character ISO 3166-1 code that identifies the country or region.
        /// </summary>
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }
}

