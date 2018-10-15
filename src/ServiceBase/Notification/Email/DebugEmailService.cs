// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;

    /// <summary>
    /// This IEmailService writes all messages as info log.
    /// </summary>
    public class DebugEmailService : IEmailService
    {
        private readonly ILogger<DefaultEmailService> _logger;

        /// <summary>
        /// Initialized the instance of <see cref="DebugEmailService"/>.
        /// </summary>
        /// <param name="logger">
        /// Instance of <see cref="ILogger{DefaultEmailService}"/>.
        /// </param>
        public DebugEmailService(ILogger<DefaultEmailService> logger)
        {
            this._logger = logger;
        }

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
        public Task SendEmailAsync(
            string templateName,
            string email,
            object viewData,
            bool sendHtml)
        {
            IDictionary<string, object> dict =
                viewData as Dictionary<string, object>;

            if (dict == null)
            {
                dict = viewData.ToDictionary();
            }

            StringBuilder sb = new StringBuilder("Sending E-Mail\n");

            sb.AppendLine(String.Format(" Template:\t{0}", templateName));
            sb.AppendLine(String.Format(" To:\t{0}", email));

            foreach (KeyValuePair<string, object> item in dict)
            {
                sb.AppendLine(
                    String.Format(" {0}:\t{1}", item.Key, item.Value));
            }

            this._logger.LogInformation(sb.ToString());

            return Task.CompletedTask;
        }
    }

}