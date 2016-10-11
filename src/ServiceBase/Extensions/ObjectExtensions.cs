using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static T ToObject<T>(this IDictionary<string, object> source) where T : class, new()
        {
            T someObject = new T();
            Type someObjectType = someObject.GetType();

            foreach (KeyValuePair<string, object> item in source)
            {
                someObjectType.GetProperty(item.Key).SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> ToDictionary(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }
    }
}
