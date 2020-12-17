namespace ServiceBase.PayPal.V2
{
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-shipping_detail
    /// </summary>
    public class ShippingDetail
    {
        /// <summary>
        /// The name of the person to whom to ship the items.
        /// Supports only the full_name property.
        /// </summary>
        [JsonProperty("name")]
        public ShippingDetailName Name { get; set; }

        /// <summary>
        /// The address of the person to whom to ship the items. Supports only
        /// the address_line_1, address_line_2, admin_area_1, admin_area_2,
        /// postal_code, and country_code properties.
        /// </summary>
        [JsonProperty("address")]
        public AddressPortable Address { get; set; }
    }
}
