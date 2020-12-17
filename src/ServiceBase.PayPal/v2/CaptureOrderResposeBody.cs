namespace ServiceBase.PayPal.V2
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-payment_collection
    /// </summary>
    public class PaymentCollection
    {
        /// <summary>
        /// An array of authorized payments for a purchase unit. A purchase
        /// unit can have zero or more authorized payments.
        /// </summary>
        [JsonProperty("authorizations")]
        public Authorization[] Authorizations { get; set; }

        /// <summary>
        /// An array of captured payments for a purchase unit.A purchase unit
        /// can have zero or more captured payments.
        /// </summary>
        [JsonProperty("captures")]
        public Capture[] Captures { get; set; }

        /// <summary>
        /// An array of refunds for a purchase unit.A purchase unit can have
        /// zero or more refunds.
        /// </summary>
        [JsonProperty("refunds")]
        public Refund[] Refunds { get; set; }
    }

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-authorization
    /// </summary>
    public class Authorization
    {

    }

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-capture
    /// </summary>
    public class Capture
    {
        /// <summary>
        /// The status of the captured payment. <see cref="CaptureStatus"/>
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// The reason why the captured payment status is PENDING or DENIED.
        /// </summary>
        [JsonProperty("status_details")]
        public CaptureStatusDetails StatusDetails { get; set; }


        /// <summary>
        /// The PayPal-generated ID for the captured payment.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-capture_status_details
    /// </summary>
    public class CaptureStatusDetails
    {
        /// <summary>
        /// The reason why the captured payment status is PENDING or DENIED.
        /// <see cref="CaptureSatusDetailsResons"/>
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }
    }

    public class CaptureSatusDetailsResons
    {
        /// <summary>
        /// The payer initiated a dispute for this captured payment with PayPal.
        /// </summary>
        public const string BUYER_COMPLAINT = "BUYER_COMPLAINT";

        /// <summary>
        /// The captured funds were reversed in response to the payer disputing
        /// this captured payment with the issuer of the financial instrument
        /// used to pay for this captured payment.
        /// </summary>
        public const string CHARGEBACK = "CHARGEBACK";

        /// <summary>
        /// The payer paid by an eCheck that has not yet cleared.
        /// </summary>
        public const string ECHECK = "ECHECK";

        /// <summary>
        /// Visit your online account. In your **Account Overview**, accept
        /// and deny this payment.
        /// </summary>
        public const string INTERNATIONAL_WITHDRAWAL
            = "INTERNATIONAL_WITHDRAWAL";

        /// <summary>
        /// No additional specific reason can be provided.For more information
        /// about this captured payment, visit your account online or contact
        /// PayPal.
        /// </summary>
        public const string OTHER = "OTHER";

        /// <summary>
        /// The captured payment is pending manual review.
        /// </summary>
        public const string PENDING_REVIEW = "PENDING_REVIEW";

        /// <summary>
        /// The payee has not yet set up appropriate receiving preferences
        /// for their account. For more information about how to accept or
        /// deny this payment, visit your account online. This reason is
        /// typically offered in scenarios such as when the currency of the
        /// captured payment is different from the primary holding currency
        /// of the payee.
        /// </summary>
        public const string RECEIVING_PREFERENCE_MANDATES_MANUAL_ACTION
            = "RECEIVING_PREFERENCE_MANDATES_MANUAL_ACTION";

        /// <summary>
        /// The captured funds were refunded.
        /// </summary>
        public const string REFUNDED = "REFUNDED";

        /// <summary>
        /// The payer must send the funds for this captured payment.This code
        /// generally appears for manual EFTs.
        /// </summary>
        public const string TRANSACTION_APPROVED_AWAITING_FUNDING
            = "TRANSACTION_APPROVED_AWAITING_FUNDING";

        /// <summary>
        /// The payee does not have a PayPal account.
        /// </summary>
        public const string UNILATERAL = "UNILATERAL";

        /// <summary>
        /// The payee's PayPal account is not verified.
        /// </summary>
        public const string VERIFICATION_REQUIRED = "VERIFICATION_REQUIRED";
    }

    /// <summary>
    /// The status of the captured payment
    /// </summary>
    public static class CaptureStatus
    {
        /// <summary>
        ///  The funds for this captured payment were credited to the payee's
        ///  PayPal account.
        /// </summary>
        public const string COMPLETED = "COMPLETED";

        /// <summary>
        /// The funds could not be captured.
        /// </summary>
        public const string DECLINED = "DECLINED";

        /// <summary>
        /// An amount less than this captured payment's amount was partially
        /// refunded to the payer.
        /// </summary>
        public const string PARTIALLY_REFUNDED = "PARTIALLY_REFUNDED";

        /// <summary>
        /// The funds for this captured payment was not yet credited to the
        /// payee's PayPal account. For more information, see status.details
        /// </summary>
        public const string PENDING = "PENDING";

        /// <summary>
        /// An amount greater than or equal to this captured payment's
        /// amount was refunded to the payer.
        /// </summary>
        public const string REFUNDED = "REFUNDED";
    }

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#definition-refund
    /// </summary>
    public class Refund
    {

    }

    /// <summary>
    /// https://developer.paypal.com/docs/api/orders/v2/#orders-capture-response
    /// </summary>
    public class CaptureOrderResposeBody
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

