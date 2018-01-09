// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System.Threading.Tasks;

    /// <summary>
    /// Sends <see cref="EmailMessage"/>.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends <see cref="EmailMessage"/>.
        /// </summary>
        /// <param name="message">The instance of
        /// <see cref="EmailMessage"/>.</param>
        Task SendEmailAsync(EmailMessage message);
    }
}