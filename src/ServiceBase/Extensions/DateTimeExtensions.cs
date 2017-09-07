using System;
using System.Globalization;

namespace ServiceBase.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly GregorianCalendar gc = new GregorianCalendar();

        public static int GetWeekOfMonth(this DateTime date)
        {
            var first = new DateTime(date.Year, date.Month, 1);
            return date.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime date)
        {
            return DateTimeExtensions.gc.GetWeekOfYear(
                date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public static DateTime GetStartOfDay(this DateTime date)
        {
            return new DateTime(
                date.Year, date.Month, date.Day,
                0, 0, 0, 0);
        }

        public static DateTime GetEndOfDay(this DateTime date)
        {
            return new DateTime(
                date.Year, date.Month, date.Day,
                23, 59, 59, 999);
        }

        public static DateTime GetStartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0);
        }

        public static DateTime GetEndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month,
              DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59, 999);
        }

        public static int GetUnixTimestamp(this DateTime date)
        {
            return (int)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}