using System;

namespace ServiceBase
{
    internal static class DateTimeHelper
    {
        public static Func<DateTime> UtcNowFunc = () => DateTime.UtcNow;

        public static DateTime UtcNow => UtcNowFunc();
    }
}
