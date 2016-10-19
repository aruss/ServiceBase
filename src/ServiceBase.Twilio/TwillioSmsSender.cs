using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBase.Notification.SMS;
using System.Threading.Tasks;

namespace ServiceBase.Notification.Twilio
{
    public class TwillioSmsSender : ISmsSender
    {
        private readonly TwillioOptions _options;
        private readonly ILogger<TwillioSmsSender> _logger;

        public TwillioSmsSender(
            IOptions<TwillioOptions> options,
            ILogger<TwillioSmsSender> logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task SendSmsAsync(string number, string message)
        {
            _logger.LogInformation($"SMS: {number} \"{message}\"");
            await Task.FromResult(0);
        }
    }
}
