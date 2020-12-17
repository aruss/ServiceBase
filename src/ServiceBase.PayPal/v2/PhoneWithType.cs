namespace ServiceBase.PayPal.V2
{
    using Newtonsoft.Json;

    /// <summary>
    /// The phone number of the customer. Available only when you enable the
    /// Contact Telephone Number option in the Profile & Settings for the
    /// merchant's PayPal account. The phone.phone_number supports only national_number.
    /// </summary>
    public class PhoneWithType
    {
        /// <summary>
        /// The phone type. Possible values: <see cref="PhoneTypes"/>
        /// </summary>
        [JsonProperty("phone_type")]
        public string PhoneType { get; set; }

        /// <summary>
        /// The phone number, in its canonical international E.164 numbering
        /// plan format.Supports only the national_number property.
        /// </summary>
        [JsonProperty("phone_number")]
        public PhoneNumber PhoneNumber { get; set; }
    }
}

