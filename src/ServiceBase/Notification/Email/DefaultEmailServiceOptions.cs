// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class DefaultEmailServiceOptions
    {
        public string DefaultLocale { get; set; }

        public string TemplateDirectoryPath { get; set; }

        public Func<HttpContext, Task<string>>
            GetTemplateDirectoryPathAsync { get; set; }

        public DefaultEmailServiceOptions()
        {
            this.GetTemplateDirectoryPathAsync = async (context) =>
            {
                return this.TemplateDirectoryPath;
            };
        }
    }
}

