// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ServiceBase.Extensions;

    public interface ITokenizer
    {
        /// <summary>
        /// Replaces template tokens with viewData
        /// </summary>
        /// <param name="template">String template.</param>
        /// <param name="data">Dictionary with data.</param>
        /// <returns>Parsed template.</returns>
        Task<string> Tokenize(string template,
            IDictionary<string, object> data);

        /// <summary>
        /// Replaces template tokens with viewData
        /// </summary>
        /// <param name="template">String template.</param>
        /// <param name="data">Object with data.</param>
        /// <returns>Parsed template.</returns>
        Task<string> Tokenize(string template, object data);
    }

    public class DefaultTokenizer : ITokenizer
    {
        /// <summary>
        /// Replaces template tokens with viewData
        /// </summary>
        /// <param name="template">String template.</param>
        /// <param name="data">Object with data.</param>
        /// <returns>Parsed template.</returns>
        public Task<string> Tokenize(string template, object data)
        {
            return this.Tokenize(template, data.ToDictionary());
        }

        /// <summary>
        /// Replaces template tokens with viewData
        /// </summary>
        /// <param name="template">String template.</param>
        /// <param name="data">Dictionary with data.</param>
        /// <returns>Parsed template.</returns>
        public Task<string> Tokenize(
            string template,
            IDictionary<string, object> data)
        {
            string result = template;
            foreach (KeyValuePair<string, object> item in data)
            {
                result = result
                    .Replace($"{{{item.Key}}}", item.Value?.ToString());
            }

            return Task.FromResult(result);
        }
    }
}
