using ServiceBase.Logging;

namespace Microsoft.Extensions.Logging
{
    public static class ILoggerExtensions
    {
        public static void LogInformation(this ILogger logger, object obj)
        {
            logger.LogInformation(LogSerializer.Serialize(obj));
        }

        public static void LogError(this ILogger logger, object obj)
        {
            logger.LogError(LogSerializer.Serialize(obj));
        }
    }
}
