// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.SendGrid
{
    using System.Threading.Tasks;
    using global::SendGrid;
    using global::SendGrid.Helpers.Mail;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using ServiceBase.Notification.Email;

    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridOptions options;
        private readonly ILogger<SendGridEmailSender> logger;

        public SendGridEmailSender(
            SendGridOptions options,
            ILogger<SendGridEmailSender> logger)
        {
            this.logger = logger;
            this.options = options;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            SendGridClient client = new SendGridClient(this.options.Key);

            SendGridMessage msg = new SendGridMessage
            {
                From = new EmailAddress(message.EmailFrom),
                Subject = message.Subject,
                PlainTextContent = message.Text,
                HtmlContent = message.Html
            };

            msg.AddTo(new EmailAddress(message.EmailTo));
            Response result = await client.SendEmailAsync(msg);

            this.logger.LogInformation(JsonConvert.SerializeObject(message));
        }
    }
}