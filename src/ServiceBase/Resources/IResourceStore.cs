// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Resources
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IResourceStore
    {
        /// <summary>
        /// Get a specifc <see cref="Resource"/> for specific culture, group
        /// and resource name
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <returns>A <see cref="Resource"/>.</returns>
        Task<Resource> GetAsync(string culture, string group, string key);

        /// <summary>
        /// Get all resources for specific culture and group.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="group"></param>
        /// <returns>A list of <see cref="Resource"/>.</returns>
        Task<IEnumerable<Resource>> GetAllAsync(string culture, string group);

        /// <summary>
        /// Returns all cultures for specific group.
        /// </summary>
        /// <param name="group"></param>
        /// <returns>A list of cultures as string.</returns>
        Task<IEnumerable<string>> GetAllCulturesAsync(string group);

        /// <summary>
        /// Adds a list of key value pairs to a specific culture and group.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="group"></param>
        /// <param name="dictionary"></param>
        /// <param name="source"></param>
        Task WriteAsync(
            string culture,
            string group,
            IDictionary<string, string> dictionary,
            string source = null);

        /// <summary>
        /// Adds a single resource
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="group"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="source"></param>
        Task WriteAsync(
            string culture,
            string group,
            string key,
            string value,
            string source = null);

        /// <summary>
        /// Removes a whole group of resources for specific culture.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="group"></param>
        /// <param name="keys"></param>
        Task DeleteAsync(
            string culture,
            string group,
            IEnumerable<string> keys,
            string source = null);
    }

}
