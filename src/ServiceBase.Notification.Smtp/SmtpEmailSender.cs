using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using ServiceBase.Notification.Email;
using System;
using System.Threading.Tasks;

namespace ServiceBase.Notification.Smtp
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(SmtpOptions options, ILogger<SmtpEmailSender> logger)
        {
            _logger = logger;
            _options = options;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var mimeMsg = new MimeMessage();
            mimeMsg.From.Add(new MailboxAddress(
                 String.IsNullOrWhiteSpace(message.EmailFrom) ?
                 _options.EmailFrom :
                 message.EmailFrom));

            mimeMsg.To.Add(new MailboxAddress(message.EmailTo));
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
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(_options.Host, _options.Port, _options.UseSsl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                if (!String.IsNullOrWhiteSpace(_options.UserName) &&
                    !String.IsNullOrWhiteSpace(_options.Password))
                {
                    client.Authenticate(_options.UserName, _options.Password);
                }

                client.Send(mimeMsg);
                client.Disconnect(true);
            }

            await Task.FromResult(0);
        }
    }
}