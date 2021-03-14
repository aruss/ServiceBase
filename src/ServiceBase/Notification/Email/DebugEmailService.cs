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

        public Task SendEmailAsync(string templateName, string email, object viewData)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailAsync(
            string templateName,
            object model,
            IEnumerable<string> emailTos = null,
            IEnumerable<string> emailCcs = null,
            IEnumerable<string> emailBccs = null)
        {
            IDictionary<string, object> dict =
               model as Dictionary<string, object>;

            if (dict == null)
            {
                dict = model.ToDictionary();
            }

            StringBuilder sb = new StringBuilder("Sending E-Mail\n");

            sb.AppendLine(String.Format(" Template:\t{0}", templateName));

            if (emailTos != null && emailTos.Count() > 0)
            {
                sb.AppendLine(
                    String.Format(" Tos:\t{0}", string.Join(", ", emailTos)));
            }

            if (emailCcs != null && emailCcs.Count() > 0)
            {
                sb.AppendLine(
                    String.Format(" Ccs:\t{0}", string.Join(", ", emailCcs)));
            }
      
            if (emailBccs != null && emailBccs.Count() > 0)
            {
                sb.AppendLine(
                    String.Format(" Bccs:\t{0}", string.Join(", ", emailBccs)));
            }

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