using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Notification.SMS
{
    public class TwillioOptions
    {
        public string User { get; set; }
        public string Key { get; set; }
    }

    public class TwillioSmsSender:  ISmsSender
    {
        private readonly TwillioOptions _options;
        private readonly ILogger<TwillioSmsSender> _logger;

        public async Task SendSmsAsync(string number, string message)
        {

            // https://docs.asp.net/en/latest/security/authentication/2fa.html#debugging-twilio
            _logger.LogInformation($"SMS: {number} \"{message}\""); 

            await Task.FromResult(0);
        }
    }    
}
