// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}