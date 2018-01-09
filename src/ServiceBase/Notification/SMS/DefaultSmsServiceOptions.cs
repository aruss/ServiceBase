// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace ServiceBase.Notification.Sms
{
    public class DefaultSmsServiceOptions
    {
        public string DefaultLocale { get; set; }

        /// <summary>
        /// Path of directory with templates.
        /// </summary>
        public string TemplateDirectoryPath { get; set; }

        public Func<string> GetTemplateDirectoryPath { get; set; }

        public DefaultSmsServiceOptions()
        {
            this.GetTemplateDirectoryPath = () =>
            {
                return this.TemplateDirectoryPath;
            };
        }
    }
}