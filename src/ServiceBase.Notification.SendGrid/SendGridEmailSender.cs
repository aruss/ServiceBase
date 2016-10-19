using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
            _logger.LogInformation(JsonConvert.SerializeObject(message));
            await Task.FromResult(0);
        }
    }
}
