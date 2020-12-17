namespace ServiceBase.PayPal.V1
{
    public class GetTokenResult
    {
        public TokenResponseBody Token { get; set; }
        public ErrorResponseBody Error { get; set; }
    }
}
