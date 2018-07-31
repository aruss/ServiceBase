// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using System;

    /// <summary>
    /// A attribute which defines a dependency onto another task.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class DependsOnPluginAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Makes the current type depend on the specified type.
        /// </summary>
        /// <param name="type">
        /// The type or interface (then it ll depend on all implementations)
        /// which the current type depends on.
        /// </param>
        public DependsOnPluginAttribute(string pluginName)
        {
            this.PluginName = pluginName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the dependency.
        /// </summary>
        public string PluginName
        {
            get;
            private set;
        }

        #endregion
    }
}
