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

    public class DebugEmailService : IEmailService
    {
        private readonly ILogger<DefaultEmailService> _logger;

        public DebugEmailService(ILogger<DefaultEmailService> logger)
        {
            this._logger = logger;
        }

        public async Task SendEmailAsync(
            string templateName, string email, object viewData, bool sendHtml)
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
        }
    }

}