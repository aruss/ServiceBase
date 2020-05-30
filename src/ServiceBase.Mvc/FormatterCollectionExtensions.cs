// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc
{
    using System;
    using Microsoft.AspNetCore.Mvc.Formatters;


    /// <summary>
    /// <see cref="FormatterCollection{IOutputFormatter}"/> extension methods.
    /// </summary>
    public static partial class FormatterCollectionExtensions
    {
        /// <summary>
        /// Adds custom configured <see cref="JsonOutputFormatter"/>.
        /// </summary>
        /// <param name="outputFormatters"></param>
        [Obsolete()]
        public static void AddDefaultJsonOutputFormatter(
            this FormatterCollection<IOutputFormatter> outputFormatters)
        {
            /*IOutputFormatter outputFormatter = outputFormatters
                .FirstOrDefault(c => c is JsonOutputFormatter);

            if (outputFormatter != null)
            {
                outputFormatters.Remove(outputFormatter);
            }

            outputFormatters.Add(new JsonOutputFormatter(
                new JsonSerializerSettings().SetupDefaults(),
                ArrayPool<Char>.Shared));
            */
        }
    }
}