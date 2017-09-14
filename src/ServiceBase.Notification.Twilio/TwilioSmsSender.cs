
namespace ServiceBase.Notification.Twilio
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Notification.Sms;

    public class TwilioSmsSender : ISmsSender
    {
        private readonly TwilioOptions _options;
        private readonly ILogger<TwilioSmsSender> _logger;

        public TwilioSmsSender(
            TwilioOptions options,
            ILogger<TwilioSmsSender> logger)
        {
            _logger = logger;
            _options = options;
        }

        public async Task SendSmsAsync(string number, string message)
        {
            _logger.LogInformation($"Send SMS to {number} \"{message}\"");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(
                        $"{_options.Sid}:{_options.Token}")));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To", number),
                    new KeyValuePair<string, string>("From", _options.From),
                    new KeyValuePair<string, string>("Body", message)
                });

                var url = $"https://api.twilio.com/2010-04-01/Accounts/" +
                    $"{_options.Sid}/Messages.json";

                var result = await client.PostAsync(url, content)
                    .ConfigureAwait(false);

                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError(result);
                }
            }
        }
    }
}