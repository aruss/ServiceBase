namespace ServiceBase.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts object to dictionary
        /// </summary>
        public static IDictionary<string, object> ToDictionary(
            this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly |
                BindingFlags.Public |
                BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }
    }
}