using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceBase.Extensions;

namespace ServiceBase.Notification.Sms
{
    public class DebugSmsService : ISmsService
    {
        private readonly ILogger<DebugSmsService> _logger;

        public DebugSmsService(ILogger<DebugSmsService> logger)
        {
            _logger = logger;
        }

        public async Task SendSmsAsync(string templateName, string number, object viewData)
        {
            IDictionary<string, object> dict = viewData as Dictionary<string, object>;
            if (dict == null)
            {
                dict = viewData.ToDictionary();
            }

            var sb = new StringBuilder();

            sb.AppendLine(String.Format("Template:\t{0}", templateName));
            sb.AppendLine(String.Format("Number:\t{0}", number));

            foreach (var item in dict)
            {
                sb.AppendLine(String.Format("\t{0}:\t{1}", item.Key, item.Value));
            }

            _logger.LogInformation(sb.ToString());
        }
    }
}