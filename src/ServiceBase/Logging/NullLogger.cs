namespace ServiceBase.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public class NullLogger<TCategoryName> : NullLogger, ILogger<TCategoryName>
    {
        public static ILogger<TCategoryName> Create()
        {
            return new NullLogger<TCategoryName>();
        }
    }

    public class NullLogger : ILogger, IDisposable
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
        }
    }
}