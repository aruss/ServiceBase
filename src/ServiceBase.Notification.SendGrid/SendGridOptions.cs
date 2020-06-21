// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.SendGrid
{
    public class SendGridOptions
    {
        public string EmailFrom { get; set; }
        public string EmailFromName { get; set; }
        public string Key { get; set; }
    }
}