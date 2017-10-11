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

    /// <summary>
    /// Twilio SMS sender
    /// </summary>
    public class TwilioSmsSender : ISmsSender
    {
        private readonly TwilioOptions _options;
        private readonly ILogger<TwilioSmsSender> _logger;

        /// <summary>
        /// Creates an instance of TwilioSmsSender
        /// </summary>
        /// <param name="options">Instance of
        /// <see cref="TwilioOptions"/></param>
        /// <param name="logger">Instance of
        /// <see cref="ILogger{TwilioSmsSender}"/></param>
        public TwilioSmsSender(
            TwilioOptions options,
            ILogger<TwilioSmsSender> logger)
        {
            _logger = logger;
            _options = options;
        }

        /// <summary>
        /// Sends SMS via Twilio service
        /// <see href="https://www.twilio.com/docs/api/messaging/send-messages"></see>
        /// </summary>
        /// <param name="numberTo">The destination phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="numberFrom">The source phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="message">The text of the message you want to send,
        /// limited to 1600 characters.</param>
        /// <returns></returns>
        public async Task SendSmsAsync(
            string numberTo,
            string numberFrom,
            string message)
        {
            if (String.IsNullOrEmpty(numberTo))
            {
                throw new ArgumentNullException(nameof(numberTo));
            }

            if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.Length > 1600)
            {
                throw new ArgumentException(
                    "The text of the message you want to send, limited to " +
                    "1600 characters.",
                    nameof(message));
            }

            if (String.IsNullOrEmpty(numberFrom))
            {
                numberFrom = this._options.From;
            }

            if (String.IsNullOrEmpty(numberFrom))
            {
                throw new ArgumentNullException(nameof(numberTo),
                    "Invalid phone number from in TwilioOptions");
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(
                        $"{_options.Sid}:{_options.Token}")));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To", numberTo),
                    new KeyValuePair<string, string>("From", numberFrom),
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