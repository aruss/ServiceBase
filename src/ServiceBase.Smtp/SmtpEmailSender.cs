using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServiceBase.Notification.Email;
using System.Threading.Tasks;

namespace ServiceBase.Notification.Smtp
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmptOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;
        
        public SmtpEmailSender(
            IOptions<SmptOptions> options,
            ILogger<SmtpEmailSender> logger)
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
