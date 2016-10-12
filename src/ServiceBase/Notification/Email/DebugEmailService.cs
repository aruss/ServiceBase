using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBase.Notification.Email
{
    public class DebugEmailService : IEmailService
    {
        private readonly ILogger<DefaultEmailService> _logger;

        public DebugEmailService(ILogger<DefaultEmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string templateName, string email, object viewData)
        {
            IDictionary<string, object> dict = viewData as Dictionary<string, object>;
            if (dict == null)
            {
                dict = viewData.ToDictionary(); 
            }

            var sb = new StringBuilder();

            sb.AppendLine(String.Format("Template:\t{0}", templateName)); 
            sb.AppendLine(String.Format("To:\t{0}", email)); 

            foreach (var item in dict)
            {
                sb.AppendLine(String.Format("\t{0}:\t{1}", item.Key, item.Value)); 
            }

            _logger.LogInformation(sb.ToString()); 
        }
    }
}
