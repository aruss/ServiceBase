namespace ServiceBase.PayPal.V2
{
    using Newtonsoft.Json;

    /// <summary>
    /// The name of the payer. Supports only the given_name and surname properties.
    /// </summary>
    public class PayerName
    {
        /// <summary>
        /// When the party is a person, the party's given, or first, name.
        /// Maximum length: 140.
        /// </summary>
        [JsonProperty("given_name ")]
        public string GivenName { get; set; }

        /// <summary>
        /// When the party is a person, the party's surname or family name.
        /// Also known as the last name. Required when the party is a person.
        /// Use also to store multiple surnames including the matronymic,
        /// or mother's, surname.
        /// Maximum length: 140.
        /// </summary>
        [JsonProperty("surname")]
        public string Surname { get; set; }

    }
}

