// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Modules
{
    using System.Collections.Generic;

    /// <summary>
    /// Module Host configuration.
    /// </summary>
    public class ModulesOptions
    {
        /// <summary>
        /// List of module configurations.
        /// </summary>
        public List<ModuleOptions> Modules { get; set; }
    }
}
