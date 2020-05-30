// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Extensions
{
    using System;
    using System.Globalization;

    public static partial class DateTimeExtensions
    {
        private static readonly GregorianCalendar gc = new GregorianCalendar();

        public static int GetWeekOfMonth(this DateTime date)
        {
            DateTime first = new DateTime(date.Year, date.Month, 1);

            return date.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        private static int GetWeekOfYear(this DateTime date)
        {
            return DateTimeExtensions.gc.GetWeekOfYear(
                date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public static DateTime GetStartOfDay(this DateTime date)
        {
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                0, 0, 0, 0,
                date.Kind
            );
        }

        public static DateTime GetEndOfDay(this DateTime date)
        {
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                23, 59, 59, 999,
                date.Kind
            );
        }

        public static DateTime GetStartOfMonth(this DateTime date)
        {
            return new DateTime(
                date.Year,
                date.Month,
                1, 0, 0, 0, 0,
                date.Kind
            );
        }

        public static DateTime GetEndOfMonth(this DateTime date)
        {
            return new DateTime(
                date.Year,
                date.Month,
                DateTime.DaysInMonth(date.Year, date.Month),
                23, 59, 59, 999,
                date.Kind
            );
        }

        public static int GetUnixTimestamp(this DateTime date)
        {
            return (int)(date.Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, date.Kind))
            ).TotalSeconds;
        }
    }
}