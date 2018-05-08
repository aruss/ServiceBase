// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System.Threading.Tasks;

    public interface IEmailService
    {
        /// <summary>
        /// Sends email
        /// </summary>
        /// <param name="templateName">
        /// Name of template will be used to parse the view data in it.
        /// </param>
        /// <param name="email">
        /// Email address of recipient.
        /// </param>
        /// <param name="viewData">
        /// The model.
        /// </param>
        /// <param name="sendHtml">
        /// If true email will be send as HTML.
        /// </param>
        Task SendEmailAsync(
            string templateName,
            string email,
            object viewData,
            bool sendHtml);
    }
}