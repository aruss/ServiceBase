using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBase
{
    internal static class DateTimeHelper
    {
        public static Func<DateTime> UtcNowFunc = () => DateTime.UtcNow;

        public static DateTime UtcNow => UtcNowFunc();
    }
}
