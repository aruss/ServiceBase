﻿// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System.Collections.Generic;

    public class EmailMessage
    {
        public IEnumerable<string> EmailTos { get; set; }
        public IEnumerable<string> EmailCcs { get; set; }
        public IEnumerable<string> EmailBccs { get; set; }
        public string EmailFrom { get; set; }
        public string Subject { get; set; }
        public string Html { get; set; }
        public string Text { get; set; }
    }
}