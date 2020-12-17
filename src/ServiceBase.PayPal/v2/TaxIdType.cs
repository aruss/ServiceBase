namespace ServiceBase.PayPal.V2
{
    /// <summary>
    /// The customer's tax ID type. Supported for the PayPal payment method only. 
    /// </summary>
    public static class TaxIdTypes
    {
        /// <summary>
        /// The individual tax ID type.
        /// </summary>
        public const string BR_CPF = "BR_CPF";

        /// <summary>
        /// The business tax ID type.
        /// </summary>
        public const string BR_CNPJ = "BR_CNPJ";
    }
}
