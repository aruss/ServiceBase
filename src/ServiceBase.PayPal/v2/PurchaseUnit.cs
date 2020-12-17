namespace ServiceBase.PayPal.V2
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-purchase_unit_request
    /// </summary>
    public class PurchaseUnit
    {
        /// <summary>
        /// The API caller-provided external ID for the purchase unit. Required for multiple
        /// purchase units when you must update the order through PATCH. If you omit this value
        /// and the order contains only one purchase unit, PayPal sets this value to default.
        /// </summary>
        [JsonProperty("reference_id")]
        [MaxLength(256)]
        public string ReferenceId { get; set; }

        /// <summary>
        /// The total order amount with an optional breakdown that provides details, such as
        /// the total item amount, total tax amount, shipping, handling, insurance,
        /// and discounts, if any.
        /// </summary>
        [JsonProperty("amount")]
        public AmountWithBreakdown Amount { get; set; }

        /// <summary>
        /// The merchant who receives payment for this transaction.
        /// </summary>
        [JsonProperty("payee")]
        public Payee Payee { get; set; }

        /// <summary>
        /// Any additional payment instructions for PayPal Commerce Platform customers.
        /// Enables features for the PayPal Commerce Platform, such as delayed disbursement and
        /// collection of a platform fee. Applies during order creation for captured payments
        /// or during capture of authorized payments.
        /// </summary>
        [JsonProperty("payment_instruction")]
        public PaymentInstruction PaymentInstruction { get; set; }

        /// <summary>
        /// The purchase description.
        /// </summary>
        [JsonProperty("description")]
        [MaxLength(127)]
        public string Description { get; set; }

        /// <summary>
        /// The API caller-provided external ID. Used to reconcile client transactions with
        /// PayPal transactions. Appears in transaction and settlement reports but is not
        /// visible to the payer.
        /// </summary>
        [JsonProperty("custom_id")]
        [MaxLength(127)]
        public string CustomId { get; set; }

        /// <summary>
        /// The API caller-provided external invoice number for this order.
        /// Appears in both the payer's transaction history and the emails that the payer receives.
        /// </summary>
        [JsonProperty("invoice_id")]
        [MaxLength(127)]
        public string InvoiceId { get; set; }

        /// <summary>
        /// The payment descriptor on account transactions on the customer's credit card
        /// statement. The maximum length of the soft descriptor is 22 characters. Of this,
        /// the PayPal prefix uses eight characters (PAYPAL *). 
        /// </summary>
        [JsonProperty("soft_descriptor")]
        [MaxLength(22)]
        public string SoftDescriptor { get; set; }

        /// <summary>
        /// An array of items that the customer purchases from the merchant.
        /// </summary>
        //[JsonProperty("items")]
        //public Items[] Items { get; set; }

        /// <summary>
        /// The name and address of the person to whom to ship the items.
        /// </summary>
        [JsonProperty("shipping")]
        public ShippingDetail Shipping { get; set; }
    }
}
