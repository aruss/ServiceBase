namespace ServiceBase.PayPal.V2
{
    /// <summary>
    /// Data returned via query params to the return address provided via 
    /// <see cref="ApplicationContext.ReturnUrl"/> or <see cref="ApplicationContext.CancelUrl"/>
    /// Example: /confirm?token=4VT6176729748013K&PayerID=N24VCX5ZJXV9N
    /// </summary>
    public class CreateOrderReturnInfo
    {
        public string Token { get; set; }
        public string PayerID { get; set; }
    }
}
