using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBase
{
    internal static class DateTimeHelper
    {
        internal static Func<DateTime> UtcNowFunc = () => DateTime.UtcNow;

        internal static DateTime UtcNow => UtcNowFunc();
    }
}
