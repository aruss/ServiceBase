namespace ServiceBase.PayPal.V2
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class ShippingDetailName
    {
        /// <summary>
        /// When the party is a person, the party's full name.
        /// </summary>
        [JsonProperty("full_name")]
        [MaxLength(300)]
        public string FullName { get; set; }
    }
}
