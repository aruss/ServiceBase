namespace ServiceBase.Mvc.Filters
{
    /// <summary>
    /// Error model used by <see cref="ExceptionFilter"/>.
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Exception type. 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Error message. 
        /// </summary>
        public object Error { get; set; }

        /// <summary>
        /// Stack trace.
        /// </summary>
        public string StackTrace { get; set; }
    }
}