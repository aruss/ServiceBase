namespace ServiceBase.PayPal.V2
{
    /// <summary>
    /// The shipping preference:
    ///  - Displays the shipping address to the customer.
    ///  - Enables the customer to choose an address on the PayPal site.
    ///  - Restricts the customer from changing the address during the payment-approval process.
    /// </summary>
    public static class ShippingPreferences
    {
        /// <summary>
        /// Redact the shipping address from the PayPal site. Recommended for 
        /// digital goods.
        /// </summary>
        public const string NoShipping = "NO_SHIPPING";

        /// <summary>
        /// Use the customer-provided shipping address on the PayPal site.
        /// </summary>
        public const string GetFromFile = "GET_FROM_FILE";

        /// <summary>
        /// Use the merchant-provided address. The customer cannot change this 
        /// address on the PayPal site.
        /// </summary>
        public const string SetProvidedAddress = "SET_PROVIDED_ADDRESS";
    }
}
