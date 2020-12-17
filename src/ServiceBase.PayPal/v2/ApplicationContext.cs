namespace ServiceBase.PayPal.V2
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-application_context
    /// </summary>
    public class ApplicationContext
    {
        /// <summary>
        /// The label that overrides the business name in the PayPal account on the PayPal site.
        /// </summary>
        [JsonProperty("brand_name")]
        [MaxLength(127)]
        public string BrandName { get; set; }

        /// <summary>
        /// The BCP 47-formatted locale of pages that the PayPal payment experience shows.
        /// PayPal supports a five-character code. <see cref="PaymentPayPal.V2.Locales"/>
        /// </summary>
        [JsonProperty("locale")]
        [MinLength(2)]
        [MaxLength(10)]
        public string Locale { get; set; }

        /// <summary>
        /// The shipping preference:
        ///  - Displays the shipping address to the customer.
        ///  - Enables the customer to choose an address on the PayPal site.
        ///  - Restricts the customer from changing the address during the payment-approval process.
        /// Default: GET_FROM_FILE
        /// <see cref="ShippingPreferences"></see>
        /// </summary>
        [JsonProperty("shipping_preference")]
        public string ShippingPreference { get; set; }

        /// <summary>
        /// The type of landing page to show on the PayPal site for customer checkout. 
        /// Default: LOGIN 
        /// <see cref="LandingPages"/>
        /// </summary>
        [JsonProperty("landing_page")]
        public string LandingPage { get; set; }

        /// <summary>
        /// Configures a Continue or Pay Now checkout flow. The possible values are:
        /// Default: CONTINUE 
        /// <see cref="UserActions"/>
        /// </summary>
        [JsonProperty("user_action")]
        public string UserAction { get; set; }

        /// <summary>
        /// The URL where the customer is redirected after the customer approves the payment.
        /// </summary>
        [JsonProperty("return_url")]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// The URL where the customer is redirected after the customer cancels the payment.
        /// </summary>
        [JsonProperty("cancel_url")]
        public string CancelUrl { get; set; }
    }
}
