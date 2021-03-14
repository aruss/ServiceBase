// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.SendGrid
{
    using System;
    using System.Threading.Tasks;
    using global::SendGrid;
    using global::SendGrid.Helpers.Mail;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using ServiceBase.Notification.Email;
    using System.Linq;

    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridOptions _options;
        private readonly ILogger<SendGridEmailSender> _logger;

        public SendGridEmailSender(
            SendGridOptions options,
            ILogger<SendGridEmailSender> logger)
        {
            this._logger = logger;
            this._options = options;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            SendGridClient client = new SendGridClient(this._options.Key);

            SendGridMessage sgMessage = new SendGridMessage
            {
                From = new EmailAddress(
                    String.IsNullOrWhiteSpace(message.EmailFrom) ?
                        this._options.EmailFrom :
                        message.EmailFrom,
                    this._options.EmailFromName
                ),

                Subject = message.Subject,
                PlainTextContent = message.Text,
                HtmlContent = message.Html
            };

            if (message.EmailTos != null && message.EmailTos.Count() > 0)
            {
                sgMessage.AddTos(message.EmailTos.Select(s => new EmailAddress(s)).ToList());
            }

            if (message.EmailCcs != null && message.EmailCcs.Count() > 0)
            {
                sgMessage.AddCcs(message.EmailCcs.Select(s => new EmailAddress(s)).ToList());
            }

            if (message.EmailBccs != null && message.EmailBccs.Count() > 0)
            {
                sgMessage.AddBccs(message.EmailBccs.Select(s => new EmailAddress(s)).ToList());
            }

            this._logger.LogDebug(sgMessage);
            Response response = await client.SendEmailAsync(sgMessage);
            this._logger.LogDebug(response);

            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Body.ReadAsStringAsync();
                this._logger.LogError("SendGrid responed with {body}", body);
            }
        }
    }
}