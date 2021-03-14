// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System.Collections.Generic;
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
        Task SendEmailAsync(
            string templateName,
            string email,
            object viewData);

        Task SendEmailAsync(
            string templateName,
            object model,
            IEnumerable<string> emailTos = null,
            IEnumerable<string> emailCcs = null,
            IEnumerable<string> emailBccs = null);
    }
}