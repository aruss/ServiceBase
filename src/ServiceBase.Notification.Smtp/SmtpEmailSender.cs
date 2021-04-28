// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Notification.Smtp
{
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Notification.Email;

    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(
            SmtpOptions options,
            ILogger<SmtpEmailSender> logger)
        {
            this._logger = logger;
            this._options = options;
        }

        /// <summary>
        /// Sends email via <see cref="SmtpClient"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(EmailMessage message)
        {
            var hasText = !string.IsNullOrWhiteSpace(message.Text);
            var hasHtml = !string.IsNullOrWhiteSpace(message.Html);

            // validate message
            if ((!hasText && !hasHtml) ||
                string.IsNullOrWhiteSpace(message.Subject) ||
                string.IsNullOrWhiteSpace(message.EmailFrom) ||
                string.IsNullOrWhiteSpace(message.EmailTo))
            {
                this._logger.LogError("Invalid EmailMessage");
                return;
            }

            using (var client = new SmtpClient()
            {
                Host = this._options.Host,
                Port = this._options.Port,
                UseDefaultCredentials = false,
                EnableSsl = this._options.UseSsl,
                Credentials = new NetworkCredential(
                this._options.UserName,
                this._options.Password
            )
            })
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(message.EmailFrom);
                    mail.Subject = message.Subject;
                    mail.BodyEncoding = Encoding.UTF8;
                    mail.SubjectEncoding = Encoding.UTF8;
                    mail.To.Add(new MailAddress(message.EmailTo));

                    // has only text 
                    if (hasText && !hasHtml)
                    {
                        mail.Body = message.Text;
                    }
                    // has only html
                    else if (hasHtml && !hasText)
                    {
                        mail.Body = message.Html;
                        mail.IsBodyHtml = true;
                    }
                    // has both
                    else
                    {
                        mail.Body = message.Text;

                        AlternateView htmlView = AlternateView
                          .CreateAlternateViewFromString(message.Html);

                        htmlView.ContentType = new ContentType("text/html");
                        mail.AlternateViews.Add(htmlView);
                    }

                    try
                    {
                        client.Send(mail);
                    }
                    catch (System.Exception ex)
                    {
                        this._logger.LogError(ex);
                    }
                }
            }
        }
    }
}
