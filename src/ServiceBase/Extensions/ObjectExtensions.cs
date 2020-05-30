// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static partial class ObjectExtensions
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