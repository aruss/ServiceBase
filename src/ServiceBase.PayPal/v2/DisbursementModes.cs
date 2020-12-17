namespace ServiceBase.PayPal.V2
{
    public static class DisbursementModes
    {
        /// <summary>
        /// The funds are released to the merchant immediately.
        /// </summary>
        public const string INSTANT = "INSTANT";

        /// <summary>
        /// The funds are held for a finite number of days. The actual duration
        /// depends on the region and type of integration. You can release the
        /// funds through a referenced payout. Otherwise, the funds disbursed
        /// automatically after the specified duration.
        /// </summary>
        public const string DELAYED = "DELAYED";
    }
}
