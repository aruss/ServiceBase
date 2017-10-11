namespace ServiceBase.Mvc
{
    public class ErrorModel
    {
        public string Type { get; set; }
        public object Error { get; set; }     
        public string StackTrace { get; set; }
    }
}
