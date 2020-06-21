namespace ServiceBase.Notification.SendGrid
{
    using System;
    using System.Threading.Tasks;
    using global::SendGrid;
    using global::SendGrid.Helpers.Mail;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using ServiceBase.Notification.Email;

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

            sgMessage.AddTo(new EmailAddress(message.EmailTo));

            this._logger.LogDebug(sgMessage);
            Response result = await client.SendEmailAsync(sgMessage);
            this._logger.LogDebug(result);
        }
    }
}