// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification
{
    using System.Collections.Generic;
    using System.IO;

    public class TextFormatter
    {
        public string Format(
            string path,
            IDictionary<string, object> viewData)
        {
            return this.TokenizeText(File.ReadAllText(path), viewData);
        }

        /// <summary>
        /// Replaces tokes with values
        /// </summary>
        /// <param name="template"></param>
        /// <param name="viewData"></param>
        /// <returns></returns>
        public string TokenizeText(
            string template,
            IDictionary<string, object> viewData)
        {
            string result = template;
            foreach (var item in viewData)
            {
                result = result
                    .Replace($"{{{item.Key}}}", item.Value.ToString());
            }

            return result;
        }
    }
}