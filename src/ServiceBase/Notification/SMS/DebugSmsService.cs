namespace ServiceBase.Notification.Sms
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;

    /// <summary>
    /// Prints SMS body into default logger.
    /// </summary>
    public class DebugSmsService : ISmsService
    {
        private readonly ILogger<DebugSmsService> _logger;

        public DebugSmsService(ILogger<DebugSmsService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sends SMS by creating a message from provided template
        /// </summary>
        /// <param name="templateName">Template name.</param>
        /// <param name="numberTo">The destination phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="viewData">Instance of the view model.</param>
        public async Task SendSmsAsync(
            string templateName,
            string numberTo,
            object viewData)
        {
            this.SendSmsAsync(templateName, numberTo, "none", viewData);
        }

        /// <summary>
        /// Sends SMS by creating a message from provided template
        /// </summary>
        /// <param name="templateName">Template name.</param>
        /// <param name="numberTo">The destination phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="numberFrom">The source phone number. Format with a
        /// '+' and country code e.g., +16175551212 (E.164 format).</param>
        /// <param name="viewData">Instance of the view model.</param>
        public async Task SendSmsAsync(
            string templateName,
            string numberTo,
            string numberFrom,
            object viewData)
        {
            IDictionary<string, object> dict =
                viewData as Dictionary<string, object>;

            if (dict == null)
            {
                dict = viewData.ToDictionary();
            }

            var sb = new StringBuilder();

            sb.AppendLine(String.Format("Template:\t{0}", templateName));
            sb.AppendLine(String.Format("Number To:\t{0}", numberTo));
            sb.AppendLine(String.Format("Number From:\t{0}", numberFrom));

            foreach (var item in dict)
            {
                sb.AppendLine(
                    String.Format("\t{0}:\t{1}", item.Key, item.Value));
            }

            _logger.LogInformation(sb.ToString());
        }
    }
}