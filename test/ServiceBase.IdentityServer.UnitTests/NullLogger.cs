using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace ServiceBase.Xunit
{
    public class NullLogger<TCategoryName> : ILogger<TCategoryName>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

        }
    }

    public class Options<TOptions> : IOptions<TOptions> where TOptions : class, new()
    {
        TOptions _options; 

        public Options(TOptions options)
        {
            _options = options; 
        }

        public TOptions Value
        {
            get
            {
                return _options; 
            }
        }
    }
}
