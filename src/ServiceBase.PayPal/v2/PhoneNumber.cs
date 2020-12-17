namespace ServiceBase.PayPal.V2
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// The phone number, in its canonical international E.164 numbering
    /// plan format.Supports only the national_number property.
    /// </summary>
    public class PhoneNumber
    {
        /// <summary>
        /// The national number, in its canonical international E.164 numbering plan format.
        /// The combined length of the country calling code(CC) and the
        /// national number must not be greater than 15 digits.The national
        /// number consists of a national destination code(NDC) and subscriber number(SN).
        /// Pattern: ^[0-9]{1,14}?$.
        /// </summary>
        [JsonProperty("national_number")]
        [MinLength(1)]
        [MaxLength(14)]
        public string NationalNumber { get; set; }
    }
}

