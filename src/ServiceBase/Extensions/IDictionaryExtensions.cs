namespace ServiceBase.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Converts dictionary to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T ToObject<T>(
            this IDictionary<string, object> source) where T : class, new()
        {
            return source.ToObject<T, object>(); 
        }

        /// <summary>
        /// Converts dictionary to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T ToObject<T>(
            this IDictionary<string, string> source) where T : class, new()
        {
            return source.ToObject<T, string>();
        }

        /// <summary>
        /// Converts dictionary to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T ToObject<T, T2>(
            this IDictionary<string, T2> source) where T : class, new()
        {
            T obj = new T();
            Type type = obj.GetType();

            foreach (KeyValuePair<string, T2> item in source)
            {
                type.GetProperty(item.Key)
                    .SetValue(obj, item.Value, null);
            }

            return obj;
        }
    }
}