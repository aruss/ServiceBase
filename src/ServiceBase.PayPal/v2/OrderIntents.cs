namespace ServiceBase.PayPal.V2
{
    /// <summary>
    /// The intent to either capture payment immediately or authorize a payment for an
    /// order after order creation. 
    /// </summary>
    public static class OrderIntents
    {
        /// <summary>
        /// The merchant intends to capture payment immediately after the customer makes
        /// a payment.
        /// </summary>
        public const string Capture = "CAPTURE";

        /// <summary>
        /// The merchant intends to authorize a payment and place funds on hold after the
        /// customer makes a payment. Authorized payments are guaranteed for up to three
        /// days but are available to capture for up to 29 days. After the three-day honor
        /// period, the original authorized payment expires and you must re-authorize the
        /// payment. You must make a separate request to capture payments on demand.
        /// </summary>
        public const string Authorize = "AUTHORIZE";
    }
}
