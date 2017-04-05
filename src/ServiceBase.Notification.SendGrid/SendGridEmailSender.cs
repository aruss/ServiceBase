using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using ServiceBase.Notification.Email;
using System.Threading.Tasks;

namespace ServiceBase.Notification.SendGrid
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridOptions _options;
        private readonly ILogger<SendGridEmailSender> _logger;

        public SendGridEmailSender(
            IOptions<SendGridOptions> options,
            ILogger<SendGridEmailSender> logger)
        {
            _logger = logger;
            _options = options.Value;
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
