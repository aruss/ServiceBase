namespace ServiceBase.PayPal.V2
{
    /// <summary>
    /// The type of landing page to show on the PayPal site for customer checkout.
    /// </summary>
    public static class LandingPages
    {
        /// <summary>
        /// When the customer clicks PayPal Checkout, the customer is redirected to a page to
        /// log in to PayPal and approve the payment.
        /// </summary>
        public const string Login = "Login";

        /// <summary>
        /// When the customer clicks PayPal Checkout, the customer is redirected to a page to
        /// enter credit or debit card and other relevant billing information required to
        /// complete the purchase.
        /// </summary>
        public const string Billing = "Billing";
    }
}
