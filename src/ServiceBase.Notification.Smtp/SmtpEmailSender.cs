﻿// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Smtp
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MailKit.Net.Smtp;
    using Microsoft.Extensions.Logging;
    using MimeKit;
    using MimeKit.Text;
    using ServiceBase.Notification.Email;

    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions options;
        private readonly ILogger<SmtpEmailSender> logger;

        public SmtpEmailSender(
            SmtpOptions options,
            ILogger<SmtpEmailSender> logger)
        {
            this.logger = logger;
            this.options = options;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            MimeMessage mimeMsg = new MimeMessage();

            mimeMsg.From.Add(new MailboxAddress(
                 String.IsNullOrWhiteSpace(message.EmailFrom) ?
                 this.options.EmailFrom :
                 message.EmailFrom));

            if (message.EmailTos != null && message.EmailTos.Count() > 0)
            {
                mimeMsg.To.AddRange(message.EmailTos.Select(s => new MailboxAddress(s)));
            }

            if (message.EmailCcs != null && message.EmailCcs.Count() > 0)
            {
                mimeMsg.To.AddRange(message.EmailCcs.Select(s => new MailboxAddress(s)));
            }

            if (message.EmailBccs != null && message.EmailBccs.Count() > 0)
            {
                mimeMsg.To.AddRange(message.EmailBccs.Select(s => new MailboxAddress(s)));
            }

            mimeMsg.Subject = message.Subject;

            if (!String.IsNullOrWhiteSpace(message.Html))
            {
                mimeMsg.Body = new TextPart(TextFormat.Html)
                {
                    Text = message.Html
                };
            }
            else if (!String.IsNullOrWhiteSpace(message.Text))
            {
                mimeMsg.Body = new TextPart(TextFormat.Text)
                {
                    Text = message.Text
                };
            }

            using (var client = new SmtpClient())
            {
#if DEBUG
                // For demo-purposes, accept all SSL certificates (in case
                // the server supports STARTTLS)
                client.ServerCertificateValidationCallback =
                    (s, c, h, e) => true;
#endif

                client.Connect(
                    this.options.Host,
                    this.options.Port,
                    this.options.UseSsl
                );

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                if (!String.IsNullOrWhiteSpace(this.options.UserName) &&
                    !String.IsNullOrWhiteSpace(this.options.Password))
                {
                    client.Authenticate(
                        this.options.UserName,
                        this.options.Password
                    );
                }

                client.Send(mimeMsg);
                client.Disconnect(true);
            }

            await Task.CompletedTask; 
        }
    }
}