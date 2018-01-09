// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServiceBase.Notification.Sms
{
    public class DefaultSmsServiceOptions
    {
        public string DefaultLocale { get; set; }

        /// <summary>
        /// Path of directory with templates.
        /// </summary>
        public string TemplateDirectoryPath { get; set; }
        
        public Func<HttpContext, Task<string>>
            GetTemplateDirectoryPathAsync { get; set; }

        public DefaultSmsServiceOptions()
        {
            this.GetTemplateDirectoryPathAsync = async (context) =>
            {
                return this.TemplateDirectoryPath;
            };
        }
    }
}