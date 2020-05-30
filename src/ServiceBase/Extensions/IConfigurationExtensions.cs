// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Microsoft.Extensions.Configuration
{
    using System;
    using System.Linq;

    /// <summary>
    /// Contains extensions for <see cref="IConfiguration"/>
    /// </summary>
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Determines whether the <see cref="IConfiguration"/> contains the
        /// specified section.
        /// </summary>
        /// <param name="config"><see cref="IConfiguration"/></param>
        /// <param name="key">The key to locate in the
        /// <see cref="IConfiguration"/></param>
        /// <returns> true if the <see cref="IConfiguration"/> contains a
        /// section with the specified key; otherwise, false.</returns>
        public static bool ContainsSection(
            this IConfiguration config,
            string key)
        {
            return config.GetChildren().Any(x => x.Key.Equals(
                key, StringComparison.OrdinalIgnoreCase));
        }
    }
}