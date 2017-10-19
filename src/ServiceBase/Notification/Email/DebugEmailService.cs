namespace ServiceBase.Notification.Email
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;

    public class DebugEmailService : IEmailService
    {
        private readonly ILogger<DefaultEmailService> logger;

        public DebugEmailService(ILogger<DefaultEmailService> logger)
        {
            this.logger = logger;
        }

        public async Task SendEmailAsync(
            string templateName, string email, object viewData, bool sendHtml)
        {
            IDictionary<string, object> dict =
                viewData as Dictionary<string, object>;

            if (dict == null)
            {
                dict = viewData.ToDictionary();
            }

            StringBuilder sb = new StringBuilder("Sending EMail\n");

            sb.AppendLine(String.Format("\tTemplate:\t{0}", templateName));
            sb.AppendLine(String.Format("\tTo:\t{0}", email));

            foreach (KeyValuePair<string, object> item in dict)
            {
                sb.AppendLine(
                    String.Format("\t{0}:\t{1}", item.Key, item.Value));
            }

            this.logger.LogInformation(sb.ToString());
        }
    }
}