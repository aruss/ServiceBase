// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using System;
    using System.Linq; 
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Manages all registered <see cref="IFrontendOptionsStore"/> instances.
    /// </summary>
    public static class IFrontendOptionsServiceExtensions 
    {
        /// <summary>
        /// Calls GetFrontendOptionsAsync on <see cref="IFrontendOptionsService"/> instances and
        /// returns them as dictionary, ready to use for the frontend.
        /// </summary>
        /// <returns>Frontend options.</returns>
        public static async Task<IDictionary<string, IDictionary<string, object>>>
            GetFrontendOptionsAsDictionaryAsync(
                this IEnumerable<IFrontendOptionsService> services,
                string[] whiteList
            )
        {
            var result = new Dictionary<string, IDictionary<string, object>>();

            foreach (var service in services)
            {
                IFrontendOptions options = await service.GetFrontendOptionsAsync();

                Type type = options.GetType();
                string name = type.Name.Replace("FrontendOptions", string.Empty);

                if (!whiteList.Any(c => c.Equals(name, StringComparison.InvariantCultureIgnoreCase))) {
                    continue; 
                }

                var valueDict = new Dictionary<string, object>();

                foreach (var propertyInfo in type.GetProperties())
                {
                    valueDict.Add(propertyInfo.Name, propertyInfo.GetValue(options));
                }

                result.Add(name, valueDict);
            }

            return result;
        }

    }
}
