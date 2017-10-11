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
            T someObject = new T();
            Type someObjectType = someObject.GetType();

            foreach (KeyValuePair<string, object> item in source)
            {
                someObjectType.GetProperty(item.Key)
                    .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }
    }
}