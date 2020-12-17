namespace ServiceBase.PayPal.V2
{
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#orders-create-request-body
    /// </summary>
    public class CreateOrderRequestBody
    {
        /// <summary>
        /// The intent to either capture payment immediately or authorize a payment
        /// for an order after order creation. 
        /// <see cref="PaymentIntents"
        /// </summary>
        [JsonProperty("intent")]
        public string Intent { get; set; }

        /// <summary>
        /// The customer who approves and pays for the order. The customer is also known as the payer.
        /// </summary>
        //[JsonProperty("payer")]
        //public Payer Payer { get; set; }

        /// <summary>
        /// Customize the payer experience during the approval process for the payment with PayPal.
        /// </summary>
        [JsonProperty("application_context")]
        public ApplicationContext ApplicationContext { get; set; }

        /// <summary>
        /// An array of purchase units. At present only 1 <see cref="PurchaseUnit"></see> is
        /// supported. Each purchase unit establishes a contract between a payer and the payee.
        /// Each purchase unit represents either a full or partial order that the payer intends
        /// to purchase from the payee.
        /// </summary>
        [JsonProperty("purchase_units")]
        public PurchaseUnit[] PurchaseUnits { get; set; }
    }
}
