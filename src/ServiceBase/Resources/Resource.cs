// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Resources
{
    using System;

    public class Resource
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The key of the resource 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value of the resource
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Culture, en-US, de-DE
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Group is like, EmailTemplates, Localization, SmsTemplates...
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Where did this resource came from.
        /// E.g. name of the plugin that installed it.
        /// </summary>
        public string Source { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}