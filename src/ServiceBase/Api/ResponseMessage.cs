namespace ServiceBase.Api
{
    public class ResponseMessage
    {
        public ResponseMessageKind Kind { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
    }
}