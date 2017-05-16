using ServiceBase.Logging;

namespace Microsoft.Extensions.Logging
{
    public static class ILoggerExtensions
    {
        public static void LogCritical(this ILogger logger, object obj)
        {
            logger.LogCritical(LogSerializer.Serialize(obj));
        }

        public static void LogDebug(this ILogger logger, object obj)
        {
            logger.LogDebug(LogSerializer.Serialize(obj));
        }

        public static void LogError(this ILogger logger, object obj)
        {
            logger.LogError(LogSerializer.Serialize(obj));
        }

        public static void LogInformation(this ILogger logger, object obj)
        {
            logger.LogInformation(LogSerializer.Serialize(obj));
        }

        public static void LogTrace(this ILogger logger, object obj)
        {
            logger.LogTrace(LogSerializer.Serialize(obj));
        }

        public static void LogWarning(this ILogger logger, object obj)
        {
            logger.LogWarning(LogSerializer.Serialize(obj));
        }
    }
}
