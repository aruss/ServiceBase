// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Email
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;

    public class DefaultEmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly DefaultEmailServiceOptions _options;
        private readonly ILogger<DefaultEmailService> _logger;
        private readonly TextFormatter _textFormatter;

        public DefaultEmailService(
            DefaultEmailServiceOptions options,
            ILogger<DefaultEmailService> logger,
            IEmailSender emailSender)
        {
            this._logger = logger;
            this._options = options;
            this._emailSender = emailSender;
            this._textFormatter = new TextFormatter();
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

            EmailMessage emailMessage = new EmailMessage
            {
                EmailTo = email,

                // TODO: implement caching
                Subject = this._textFormatter.Format(
                    Path.Combine(this._options.TemplateDirectoryPath,
                    $"{templateName}_Subject.txt"),
                    dict
                )
            };

            if (sendHtml)
            {
                // TODO: implement razor parsing
                emailMessage.Html = this._textFormatter.Format(
                   Path.Combine(this._options.TemplateDirectoryPath,
                   $"{templateName}_Body.cshtml"),
                   dict);
            }
            else
            {
                emailMessage.Text = this._textFormatter.Format(
                    Path.Combine(this._options.TemplateDirectoryPath,
                    $"{templateName}_Body.txt"),
                    dict);
            }

            await this._emailSender.SendEmailAsync(emailMessage);
        }
    }
}