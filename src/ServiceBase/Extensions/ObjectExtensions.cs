using System;
using System.Linq;

namespace ServiceBase
{
    public static class StringExtensions
    {
        private static readonly string[] Booleans = new string[] { "true", "yes", "on", "1" };

        public static bool AsBool(this string value, bool defaultValue = false)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return Booleans.Contains(value.ToString().ToLower());
            }

            return defaultValue;
        }
    }
}
