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
        public static TResult ToObject<TResult>(
            this IDictionary<string, object> source)
            where TResult : class, new()
        {
            return source.ToObject<TResult, object>();
        }

        /// <summary>
        /// Converts dictionary to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TResult ToObject<TResult>(
            this IDictionary<string, string> source)
            where TResult : class, new()
        {
            return source.ToObject<TResult, string>();
        }

        /// <summary>
        /// Converts dictionary to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TResult ToObject<TResult, TValue>(
            this IDictionary<string, TValue> source)
            where TResult : class, new()
        {
            TResult obj = new TResult();
            Type type = obj.GetType();

            foreach (KeyValuePair<string, TValue> item in source)
            {
                try
                {
                    type
                        .GetProperty(item.Key)
                        .SetValue(obj, item.Value, null);
                }
                catch (Exception)
                {
                }
            }

            return obj;
        }
    }
}