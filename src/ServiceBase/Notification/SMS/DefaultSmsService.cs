namespace ServiceBase.Notification.Sms
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;

    public class DefaultSmsService : ISmsService
    {
        private readonly ISmsSender _smsSender;
        private readonly DefaultSmsServiceOptions _options;
        private readonly ILogger<DefaultSmsService> _logger;
        private readonly TextFormatter _textFormatter;

        public DefaultSmsService(
            DefaultSmsServiceOptions options,
            ILogger<DefaultSmsService> logger,
            ISmsSender smsSender)
        {
            _logger = logger;
            _options = options;
            _smsSender = smsSender;
            _textFormatter = new TextFormatter();
        }

        public async Task SendSmsAsync(
            string templateName, string number, object viewData)
        {
            IDictionary<string, object> dict =
                viewData as Dictionary<string, object>;

            if (dict == null)
            {
                dict = viewData.ToDictionary();
            }

            var message = _textFormatter.Format(
                Path.Combine(
                    _options.TemplateDirectoryPath, $"{templateName}.txt"),
                dict);

            await _smsSender.SendSmsAsync(number, message);
        }
    }
}