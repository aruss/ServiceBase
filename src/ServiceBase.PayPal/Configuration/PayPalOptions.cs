namespace ServiceBase.PayPal
{
    /// <summary>
    /// 
    /// </summary>
    public class PayPalOptions
    {
        /// <summary>
        /// Default client id, is only used in static mode 
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Default secret, is only used in static mode
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// PayPal services base url, is sandbox on test environments 
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
