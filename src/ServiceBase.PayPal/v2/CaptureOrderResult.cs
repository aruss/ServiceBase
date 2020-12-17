namespace ServiceBase.PayPal.V2
{
    public class CaptureOrderResult
    {
        public CaptureOrderResposeBody Order { get; set; }
        public ErrorResponseBody Error { get; set; }
    }
}
