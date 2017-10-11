namespace ServiceBase
{
    using System;

    /// <summary>
    /// Datetime helper for abstracting UtcNow for unit tests and stuff
    /// </summary>
    public interface IDateTimeAccessor
    {
        DateTime UtcNow { get; }
    }

    /// <summary>
    /// Default impmentation of <see cref="IDateTimeAccessor"/>
    /// </summary>
    public class DateTimeAccessor : IDateTimeAccessor
    {
        public DateTime UtcNow => UtcNowFunc();
        public Func<DateTime> UtcNowFunc = () => DateTime.UtcNow;
    }
}