namespace ServiceBase.PayPal.V2
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#orders-create-response
    /// </summary>
    public class CreateOrderResponseBody
    {
        /// <summary>
        /// The ID of the order.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The date and time when the transaction occurred.
        /// </summary>
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// The date and time when the transaction was last updated.
        /// </summary>
        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// The intent to either capture payment immediately or authorize a payment for an order
        /// after order creation.
        /// <see cref="PaymentIntents"
        /// </summary>
        [JsonProperty("intent")]
        public string Intent { get; set; }

        /// <summary>
        /// The customer who approves and pays for the order. The customer is also known as the
        /// payer.
        /// </summary>
        [JsonProperty("payer")]
        public Payer Payer { get; set; }

        /// <summary>
        /// An array of purchase units. At present only 1 <see cref="PurchaseUnit"></see> is
        /// supported. Each purchase unit establishes a contract between a payer and the payee.
        /// Each purchase unit represents either a full or partial order that the payer intends
        /// to purchase from the payee.
        /// </summary>
        [JsonProperty("purchase_units")]
        public PurchaseUnit[] PurchaseUnits { get; set; }

        /// <summary>
        /// The order status. The possible values are:
        ///  - CREATED. The order was created with the specified context.
        ///  - SAVED. The order was saved and persisted.The order status continues to be in progress
        ///    until a capture is made with final_capture = true for all purchase units within the
        ///    order.
        ///  - APPROVED. The customer approved the payment through the PayPal wallet or another
        ///    form of guest or unbranded payment. For example, a card, bank account, or so on.
        ///  - VOIDED. All purchase units in the order are voided.
        ///  - COMPLETED. The payment was authorized or the authorized payment was captured for the order.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// An array of request-related HATEOAS links. To complete payer approval, use the
        /// approve link with the GET method.
        /// </summary>
        [JsonProperty("links")]
        public LinkDescription[] Links { get; set; }
    }
}

