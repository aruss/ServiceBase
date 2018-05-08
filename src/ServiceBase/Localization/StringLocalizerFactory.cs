// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Localization
{
    using System;
    using Microsoft.Extensions.Localization;

    /// <summary>
    /// Represents a factory that creates <see cref="IStringLocalizer"/>
    /// instances.
    /// It does not create anything it just returns always the same
    /// instance of <see cref="IStringLocalizer"/>.
    /// </summary>
    public class StringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IStringLocalizer _stringLocalizer;

        /// <summary>
        /// Initializes <see cref="StringLocalizerFactory"/> instance.
        /// </summary>
        /// <param name="stringLocalizer">
        /// Instance of <see cref="IStringLocalizer"/>.
        /// </param>
        public StringLocalizerFactory(IStringLocalizer stringLocalizer)
        {
            this._stringLocalizer = stringLocalizer;
            
        }

        /// <summary>
        ///  Creates an instance of <see cref="IStringLocalizer"/>.
        /// </summary>
        /// <param name="resourceSource">The System.Type.</param>
        /// <returns>The <see cref="IStringLocalizer"/>.</returns>
        public IStringLocalizer Create(Type resourceSource)
        {
            return this._stringLocalizer; 
        }

        /// <summary>
        ///  Creates an instance of <see cref="IStringLocalizer"/>.
        /// </summary>
        /// <param name="baseName">
        /// The base name of the resource to load strings from.
        /// </param>
        /// <param name="location">
        /// The location to load resources from.
        /// </param>
        /// <returns>The <see cref="IStringLocalizer"/>.</returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            return this._stringLocalizer; 
        }
    }
}
