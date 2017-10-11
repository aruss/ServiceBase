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
        private readonly SendGridOptions _options;
        private readonly ILogger<SendGridEmailSender> _logger;

        public SendGridEmailSender(
            SendGridOptions options,
            ILogger<SendGridEmailSender> logger)
        {
            _logger = logger;
            _options = options;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var client = new SendGridClient(_options.Key);
            var msg = new SendGridMessage
            {
                From = new EmailAddress(message.EmailFrom),
                Subject = message.Subject,
                PlainTextContent = message.Text,
                HtmlContent = message.Html
            };
            msg.AddTo(new EmailAddress(message.EmailTo));
            var result = await client.SendEmailAsync(msg);

            _logger.LogInformation(JsonConvert.SerializeObject(message));
        }
    }
}